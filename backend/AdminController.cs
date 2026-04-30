using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAgency.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(AppDbContext db, PasswordService passwordService) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers() => Ok(await db.Users.ToListAsync());

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(User model)
    {
        model.Id = 0;
        model.PasswordHash = passwordService.HashPassword(model.PasswordHash);
        db.Users.Add(model);
        await db.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPut("users/{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, User model)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.Phone = model.Phone;
        user.Role = model.Role;
        if (!string.IsNullOrWhiteSpace(model.PasswordHash)) user.PasswordHash = passwordService.HashPassword(model.PasswordHash);
        await db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
