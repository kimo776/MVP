using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAgency.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductOrdersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetAll(string? search, ShipmentType? shipmentType, CurrencyType? currency, ProductStatus? status)
    {
        var query = db.ProductOrders.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.ProductName.Contains(search) || x.CustomerName.Contains(search));
        if (shipmentType.HasValue) query = query.Where(x => x.PreferredShipmentType == shipmentType);
        if (currency.HasValue) query = query.Where(x => x.Currency == currency);
        if (status.HasValue) query = query.Where(x => x.ProductStatus == status);
        return Ok(await query.OrderByDescending(x => x.Id).ToListAsync());
    }

    [HttpGet("my/{customerId:int}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMine(int customerId) => Ok(await db.ProductOrders.Where(x => x.CustomerId == customerId).ToListAsync());

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(ProductOrder model)
    {
        model.Id = 0;
        model.ProductStatus = ProductStatus.WaitingPickup;
        model.RequestedDate = DateTime.UtcNow;
        db.ProductOrders.Add(model);
        await db.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Update(int id, ProductOrder model)
    {
        var entity = await db.ProductOrders.FindAsync(id);
        if (entity == null) return NotFound();
        entity.ProductName = model.ProductName;
        entity.CustomerName = model.CustomerName;
        entity.CustomerPhone = model.CustomerPhone;
        entity.PickupAddress = model.PickupAddress;
        entity.ProductPrice = model.ProductPrice;
        entity.Currency = model.Currency;
        entity.PreferredShipmentType = model.PreferredShipmentType;
        entity.ProductStatus = model.ProductStatus;
        await db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.ProductOrders.FindAsync(id);
        if (entity == null) return NotFound();
        db.ProductOrders.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
