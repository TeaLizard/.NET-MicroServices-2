using OrderService.Models;
using System.Text;
using System.Text.Json;

namespace OrderService.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient http, ILogger<ProductServiceClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<ProductDto?> GetProductAsync(int productId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/products/{productId}");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product {ProductId}", productId);
                return null;
            }
        }

        public async Task<ProductDto?> DeductStockAsync(int productId, int quantity)
        {
            try
            {
                var content = new StringContent(quantity.ToString(), Encoding.UTF8, "application/json");
                var response = await _http.PostAsync($"/api/products/{productId}/deduct-stock", content);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deduct stock for product {ProductId}", productId);
                return null;
            }
        }
    }
}
