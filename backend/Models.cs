namespace DeliveryAgency.Api;

public enum UserRole { Admin, Customer, Employee, Delivery }
public enum ProductStatus { WaitingPickup, PickupAssigned, PickedUp, ReadyForShipment, ShipmentCreated, Cancelled }
public enum TaskStatus { Assigned, InProgress, PickedUp, DeliveredToStorage, Completed, Cancelled }
public enum ShipmentType { Air, Sea }
public enum ShipmentStatus { Created, Processing, Sent, Delivered, Cancelled }
public enum CurrencyType { USD, EUR, RUB, CNY }

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

public class ProductOrder
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public CurrencyType Currency { get; set; }
    public ShipmentType PreferredShipmentType { get; set; }
    public DateTime RequestedDate { get; set; }
    public ProductStatus ProductStatus { get; set; }
}

public class DeliveryTask
{
    public int Id { get; set; }
    public int ProductOrderId { get; set; }
    public int DeliveryUserId { get; set; }
    public string PickupAddress { get; set; } = string.Empty;
    public string DestinationPoint { get; set; } = "A8";
    public TaskStatus TaskStatus { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
}

public class Shipment
{
    public int Id { get; set; }
    public int ProductOrderId { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public ShipmentType ShipmentType { get; set; }
    public CurrencyType Currency { get; set; }
    public decimal BasePrice { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime ShipmentDate { get; set; }
    public ShipmentStatus ShipmentStatus { get; set; }
    public int CreatedByUserId { get; set; }
}
