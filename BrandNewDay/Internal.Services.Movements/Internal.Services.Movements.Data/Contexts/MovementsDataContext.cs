using Microsoft.EntityFrameworkCore;
using Internal.Services.Movements.Data.Models;
using Internal.Services.Movements.Data.Models.Enums;

namespace Internal.Services.Movements.Data.Contexts
{
    public class MovementsDataContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCustomer> ProductsCustomers { get; set; }

        public MovementsDataContext(DbContextOptions<MovementsDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(x => x.ProductType)
                .HasConversion(
                    v => v.ToString(),
                    v => (EnumProductType)Enum.Parse(typeof(EnumProductType), v));

            modelBuilder.Entity<Product>()
                .HasMany(a => a.ProductCustomers)
                .WithOne(b => b.Product)
                .HasForeignKey(b => b.ProductId);

            modelBuilder.Entity<Customer>()
                .HasMany(a => a.ProductCustomers)
                .WithOne(b => b.Customer)
                .HasForeignKey(b => b.CustomerId);
        }
    }
}
