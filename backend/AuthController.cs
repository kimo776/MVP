using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAgency.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, PasswordService passwordService, JwtService jwtService) : ControllerBase
{
    public class LoginRequest { public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !passwordService.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password");

        return Ok(new { token = jwtService.GenerateToken(user), userId = user.Id, fullName = user.FullName, role = user.Role.ToString() });
    }
}
