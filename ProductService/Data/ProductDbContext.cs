using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
    
            // Seed initial data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "A high-performance laptop.", Price = 999.99m, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Description = "A latest model smartphone.", Price = 499.99m, Stock = 20 },
                new Product { Id = 3, Name = "Headphones", Description = "Noise-cancelling headphones.", Price = 199.99m, Stock = 30 }
            );
    }
}