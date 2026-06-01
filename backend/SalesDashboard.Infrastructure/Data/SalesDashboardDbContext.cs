using Microsoft.EntityFrameworkCore;
using SalesDashboard.Domain.Entities;

namespace SalesDashboard.Infrastructure.Data;

public class SalesDashboardDbContext : DbContext
{
    public SalesDashboardDbContext(DbContextOptions<SalesDashboardDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);
    }
}