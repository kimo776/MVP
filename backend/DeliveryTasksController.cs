using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAgency.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryTasksController(AppDbContext db) : ControllerBase
{
    public class UpdateTaskStatusRequest { public TaskStatus Status { get; set; } public string? Notes { get; set; } }

    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetAll() => Ok(await db.DeliveryTasks.ToListAsync());

    [HttpGet("my/{deliveryUserId:int}")]
    [Authorize(Roles = "Delivery")]
    public async Task<IActionResult> GetMy(int deliveryUserId) => Ok(await db.DeliveryTasks.Where(x => x.DeliveryUserId == deliveryUserId).ToListAsync());

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Create(DeliveryTask model)
    {
        var order = await db.ProductOrders.FindAsync(model.ProductOrderId);
        if (order == null) return BadRequest("Product order not found");
        model.Id = 0;
        model.AssignedDate = DateTime.UtcNow;
        model.TaskStatus = TaskStatus.Assigned;
        db.DeliveryTasks.Add(model);
        order.ProductStatus = ProductStatus.PickupAssigned;
        await db.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin,Employee,Delivery")]
    public async Task<IActionResult> PatchStatus(int id, UpdateTaskStatusRequest request)
    {
        var task = await db.DeliveryTasks.FindAsync(id);
        if (task == null) return NotFound();

        task.TaskStatus = request.Status;
        task.Notes = request.Notes ?? task.Notes;
        if (request.Status is TaskStatus.Completed or TaskStatus.DeliveredToStorage) task.CompletedDate = DateTime.UtcNow;

        var order = await db.ProductOrders.FindAsync(task.ProductOrderId);
        if (order != null)
        {
            if (request.Status == TaskStatus.PickedUp) order.ProductStatus = ProductStatus.PickedUp;
            if (request.Status is TaskStatus.DeliveredToStorage or TaskStatus.Completed) order.ProductStatus = ProductStatus.ReadyForShipment;
        }

        await db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.DeliveryTasks.FindAsync(id);
        if (entity == null) return NotFound();
        db.DeliveryTasks.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
