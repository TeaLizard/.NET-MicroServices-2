using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    [Required]
    [MinLength(1)]
    public string ProductName { get; set; } = string.Empty;
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}