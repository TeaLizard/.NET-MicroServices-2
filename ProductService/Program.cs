using Microsoft.EntityFrameworkCore;
using ProductService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite("Data Source=products.db"));

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new ProductService.Models.Product { Name = "Laptop", Description = "16GB RAM, 512GB SSD", Price = 999.99m, Stock = 10 },
            new ProductService.Models.Product { Name = "Mouse", Description = "Wireless ergonomic", Price = 29.99m, Stock = 50 },
            new ProductService.Models.Product { Name = "Keyboard", Description = "Mechanical RGB", Price = 79.99m, Stock = 30 }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();