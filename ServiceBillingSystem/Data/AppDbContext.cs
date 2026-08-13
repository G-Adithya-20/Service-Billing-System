using Microsoft.EntityFrameworkCore;
using ServiceBillingSystem.Models;
namespace ServiceBillingSystem.Data;
public class AppDbContext : DbContext
{
   public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillItem> BillItems { get; set; }
    public DbSet<Company> Companies { get; set; }
}