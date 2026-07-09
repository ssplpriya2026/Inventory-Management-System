using Inventory_Management_System.Models;

namespace Inventory_Management_System.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetLowStockAsync();
        Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null);
    }
}
