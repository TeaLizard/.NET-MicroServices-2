using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;

string productBase = "http://localhost:5001/api/products";
string orderBase = "http://localhost:5002/api/orders";

var http = new HttpClient();
var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };

Console.WriteLine("Demo Client\n");

// list products
Console.WriteLine("Products:");
var products = await http.GetFromJsonAsync<List<Product>>(productBase, opts);
foreach (var p in products!)
{
    Console.WriteLine($"  ID:{p.Id} | {p.Name} | ${p.Price} | Stock:{p.Stock}");
}

// create order
Console.WriteLine("\nCreating order for 2 units of product ID 1...");
var orderRequest = new
{
    ProductId = 1,
    Quantity = 2
};
var orderResponse = await http.PostAsJsonAsync(orderBase, orderRequest, opts);
if (orderResponse.IsSuccessStatusCode)
{
    var createdOrder = await orderResponse.Content.ReadFromJsonAsync<Order>(opts);
    Console.WriteLine($"Order created successfully: ID {createdOrder?.Id}");
}
else
{
    Console.WriteLine("Error creating order.");
}

// show updated products
Console.WriteLine("\nUpdated Products:");
var updatedProducts = await http.GetFromJsonAsync<List<Product>>(productBase, opts);
foreach (var p in updatedProducts!)
{
    Console.WriteLine($"  ID:{p.Id} | {p.Name} | ${p.Price} | Stock:{p.Stock}");
}

Console.WriteLine("\nDemo complete.");

internal record Product(int Id, string Name, string Description, decimal Price, int Stock, DateTime CreatedAt);

internal record Order(int Id, int ProductId, string ProductName, int Quantity, decimal TotalPrice, string Status, DateTime CreatedAt);
