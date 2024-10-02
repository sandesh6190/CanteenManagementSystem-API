using Canteen.Data.Entities;
using Lib.Authorization.Data;
using Microsoft.EntityFrameworkCore;

namespace Canteen.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : AuthDbContext<ApplicationDbContext>(options)
{
    public DbSet<Unit> Units { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RedeemType> RedeemTypes { get; set; }
    public DbSet<Redeem> Redeems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<UserBalance> UserBalances { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationStatus> NotificationStatuses { get; set; }
}
