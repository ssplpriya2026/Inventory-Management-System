using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.Data;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetLowStockAsync()
        {
            return await _dbSet
                .Where(p => p.CurrentStockQuantity <= p.ReorderThreshold)
                .ToListAsync();
        }

        public async Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null)
        {
            return await _dbSet.AnyAsync(p => p.SKU == sku && (excludeProductId == null || p.Id != excludeProductId));
        }
    }
}
