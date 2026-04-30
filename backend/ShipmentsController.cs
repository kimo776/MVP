using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAgency.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentsController(AppDbContext db, PriceService priceService) : ControllerBase
{
    public class CreateShipmentRequest
    {
        public int ProductOrderId { get; set; }
        public ShipmentType ShipmentType { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal BasePrice { get; set; }
        public int CreatedByUserId { get; set; }
    }

    public class UpdateShipmentStatusRequest { public ShipmentStatus Status { get; set; } }

    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> GetAll(string? search, ShipmentType? shipmentType, CurrencyType? currency, ShipmentStatus? status)
    {
        var query = db.Shipments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.ShipmentNumber.Contains(search));
        if (shipmentType.HasValue) query = query.Where(x => x.ShipmentType == shipmentType);
        if (currency.HasValue) query = query.Where(x => x.Currency == currency);
        if (status.HasValue) query = query.Where(x => x.ShipmentStatus == status);
        return Ok(await query.OrderByDescending(x => x.Id).ToListAsync());
    }

    [HttpGet("my/{customerId:int}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMy(int customerId)
    {
        var orderIds = await db.ProductOrders.Where(x => x.CustomerId == customerId).Select(x => x.Id).ToListAsync();
        return Ok(await db.Shipments.Where(x => orderIds.Contains(x.ProductOrderId)).ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Create(CreateShipmentRequest model)
    {
        var order = await db.ProductOrders.FindAsync(model.ProductOrderId);
        if (order == null) return BadRequest("Product order not found");
        if (order.ProductStatus != ProductStatus.ReadyForShipment) return BadRequest("Shipment can be created only when product is ReadyForShipment");

        var count = await db.Shipments.CountAsync();
        var shipmentNumber = $"SHP-{(count + 1).ToString("D4")}";
        var (deliveryFee, totalPrice) = priceService.Calculate(model.ShipmentType, model.BasePrice);

        var shipment = new Shipment
        {
            ProductOrderId = model.ProductOrderId,
            ShipmentNumber = shipmentNumber,
            ShipmentType = model.ShipmentType,
            Currency = model.Currency,
            BasePrice = model.BasePrice,
            DeliveryFee = deliveryFee,
            TotalPrice = totalPrice,
            ShipmentDate = DateTime.UtcNow,
            ShipmentStatus = ShipmentStatus.Created,
            CreatedByUserId = model.CreatedByUserId
        };

        db.Shipments.Add(shipment);
        order.ProductStatus = ProductStatus.ShipmentCreated;
        await db.SaveChangesAsync();
        return Ok(shipment);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Update(int id, Shipment model)
    {
        var entity = await db.Shipments.FindAsync(id);
        if (entity == null) return NotFound();
        entity.ShipmentType = model.ShipmentType;
        entity.Currency = model.Currency;
        entity.BasePrice = model.BasePrice;
        var calc = priceService.Calculate(model.ShipmentType, model.BasePrice);
        entity.DeliveryFee = calc.deliveryFee;
        entity.TotalPrice = calc.totalPrice;
        entity.ShipmentStatus = model.ShipmentStatus;
        await db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> PatchStatus(int id, UpdateShipmentStatusRequest model)
    {
        var entity = await db.Shipments.FindAsync(id);
        if (entity == null) return NotFound();
        entity.ShipmentStatus = model.Status;
        await db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Shipments.FindAsync(id);
        if (entity == null) return NotFound();
        db.Shipments.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
