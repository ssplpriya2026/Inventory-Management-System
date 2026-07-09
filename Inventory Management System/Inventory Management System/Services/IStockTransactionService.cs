using Inventory_Management_System.ViewModel;

namespace Inventory_Management_System.Services
{
    public interface IStockTransactionService
    {
        Task<StockTransactionViewModel> GetCreateFormAsync();
        Task<ServiceResult> ProcessStockMovementAsync(StockTransactionViewModel viewModel);
        Task<List<RecentStockTransactionViewModel>> GetRecentTransactionAsync();
    }
}
