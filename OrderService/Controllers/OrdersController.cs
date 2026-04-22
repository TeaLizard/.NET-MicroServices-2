using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly ProductServiceClient _productClient;

    public OrdersController(OrderDbContext db, ProductServiceClient productClient)
    {
        _db = db;
        _productClient = productClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Orders.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        return order == null ? NotFound(new { message = $"Order {id} not found" }) : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        // check for product
        var product = await _productClient.GetProductAsync(request.ProductId);
        if (product is null)
            return NotFound(new { message = $"Product {request.ProductId} not found in ProductService" });

        // check stock
        if (product.Stock < request.Quantity)
            return BadRequest(new { message = $"Insufficient stock. Available: {product.Stock}, Requested: {request.Quantity}" });

        // deduct stock
        var updated = await _productClient.DeductStockAsync(request.ProductId, request.Quantity);
        if (updated == null)
            return StatusCode(502, new { message = "Failed to reserve stock in ProductService" });

        // create order
        var order = new Order
        {
            ProductId = request.ProductId,
            ProductName = product.Name,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            Status = "Confirmed"
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // archive/cancel order
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = "Cancelled";
        await _db.SaveChangesAsync();
        return Ok(order);
    }
}