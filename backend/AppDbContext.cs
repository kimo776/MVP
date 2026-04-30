using Microsoft.EntityFrameworkCore;

namespace DeliveryAgency.Api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ProductOrder> ProductOrders => Set<ProductOrder>();
    public DbSet<DeliveryTask> DeliveryTasks => Set<DeliveryTask>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
}
