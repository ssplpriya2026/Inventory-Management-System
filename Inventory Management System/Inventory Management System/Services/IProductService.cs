using Inventory_Management_System.ViewModel;

namespace Inventory_Management_System.Services
{
    public interface IProductService
    {
        Task<List<ProductViewModel>> GetAllAsync();
        Task<ProductViewModel> GetByIdAsync(int id);
        Task<List<ProductViewModel>> GetLowStockAsync();
        Task<ProductViewModel> GetCreateFormAsync();
        Task<ProductViewModel> GetEditFormAsync(int id);
        Task<ServiceResult> CreateAsync(ProductViewModel viewModel);
        Task<ServiceResult> UpdateAsync(ProductViewModel viewModel);
        Task<ServiceResult> DeleteAsync(int id);
        Task<DashBoardViewModel> GetDashBoardAsync();
        Task<StockValueReportViewModel> GetStockValueReportAsync();
    }
}
