using Inventory_Management_System.Data;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.Repository
{
    public class StockTransactionRepository : Repository<StockTransaction> , IStockTransactionRepository
    {
        public StockTransactionRepository(ApplicationDbContext context) : base(context)
        { 
        }
    }
}
