using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Products
            builder.Entity<Product>().HasIndex(p => p.ProductCode).IsUnique();
            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");

            // Stocks
            builder.Entity<Stock>().HasIndex(s => s.StockCode).IsUnique();
        }
    }
}
