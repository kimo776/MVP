namespace DeliveryAgency.Api;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db, PasswordService passwordService)
    {
        if (db.Users.Any()) return;

        var users = new List<User>
        {
            new() { FullName = "System Admin", Email = "admin@example.com", Phone = "+1000000001", PasswordHash = passwordService.HashPassword("Admin123!"), Role = UserRole.Admin },
            new() { FullName = "John Customer", Email = "customer@example.com", Phone = "+1000000002", PasswordHash = passwordService.HashPassword("Customer123!"), Role = UserRole.Customer },
            new() { FullName = "Emma Employee", Email = "employee@example.com", Phone = "+1000000003", PasswordHash = passwordService.HashPassword("Employee123!"), Role = UserRole.Employee },
            new() { FullName = "David Delivery", Email = "delivery@example.com", Phone = "+1000000004", PasswordHash = passwordService.HashPassword("Delivery123!"), Role = UserRole.Delivery }
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        var customer = users.First(u => u.Role == UserRole.Customer);
        var employee = users.First(u => u.Role == UserRole.Employee);
        var delivery = users.First(u => u.Role == UserRole.Delivery);

        var orders = new List<ProductOrder>
        {
            new() { ProductName = "Laptop", CustomerId = customer.Id, CustomerName = customer.FullName, CustomerPhone = customer.Phone, PickupAddress = "12 Main St", ProductPrice = 900, Currency = CurrencyType.USD, PreferredShipmentType = ShipmentType.Air, RequestedDate = DateTime.UtcNow.AddDays(-2), ProductStatus = ProductStatus.ReadyForShipment },
            new() { ProductName = "Clothes Box", CustomerId = customer.Id, CustomerName = customer.FullName, CustomerPhone = customer.Phone, PickupAddress = "45 River Rd", ProductPrice = 300, Currency = CurrencyType.EUR, PreferredShipmentType = ShipmentType.Sea, RequestedDate = DateTime.UtcNow.AddDays(-1), ProductStatus = ProductStatus.PickupAssigned },
            new() { ProductName = "Phone Accessories", CustomerId = customer.Id, CustomerName = customer.FullName, CustomerPhone = customer.Phone, PickupAddress = "78 Hill Ave", ProductPrice = 120, Currency = CurrencyType.RUB, PreferredShipmentType = ShipmentType.Air, RequestedDate = DateTime.UtcNow, ProductStatus = ProductStatus.WaitingPickup }
        };
        db.ProductOrders.AddRange(orders);
        await db.SaveChangesAsync();

        db.DeliveryTasks.AddRange(
            new DeliveryTask { ProductOrderId = orders[1].Id, DeliveryUserId = delivery.Id, PickupAddress = orders[1].PickupAddress, DestinationPoint = "A8", TaskStatus = TaskStatus.Assigned, AssignedDate = DateTime.UtcNow.AddDays(-1), Notes = "Handle with care" },
            new DeliveryTask { ProductOrderId = orders[0].Id, DeliveryUserId = delivery.Id, PickupAddress = orders[0].PickupAddress, DestinationPoint = "A8", TaskStatus = TaskStatus.Completed, AssignedDate = DateTime.UtcNow.AddDays(-2), CompletedDate = DateTime.UtcNow.AddDays(-1), Notes = "Delivered to storage" }
        );

        db.Shipments.AddRange(
            new Shipment { ProductOrderId = orders[0].Id, ShipmentNumber = "SHP-0001", ShipmentType = ShipmentType.Air, Currency = CurrencyType.USD, BasePrice = 100, DeliveryFee = 15, TotalPrice = 115, ShipmentDate = DateTime.UtcNow.AddDays(-1), ShipmentStatus = ShipmentStatus.Processing, CreatedByUserId = employee.Id }
        );

        await db.SaveChangesAsync();
    }
}
