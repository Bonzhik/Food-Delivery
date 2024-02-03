using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using wep_api_food.Models;

namespace wep_api_food.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>()
                .HasKey(pc => new { pc.OrderId, pc.ProductId });
            modelBuilder.Entity<OrderProduct>()
                .HasOne(p => p.Order)
                .WithMany(pc => pc.OrderProducts)
                .HasForeignKey(p => p.OrderId);
            modelBuilder.Entity<OrderProduct>()
                .HasOne(p => p.Product)
                .WithMany(pc => pc.OrderProducts)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
