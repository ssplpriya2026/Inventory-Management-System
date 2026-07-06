using Inventory_Management_System.ViewModel;

namespace Inventory_Management_System.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel?> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(CategoryViewModel viewModel);
        Task<ServiceResult> UpdateAsync(CategoryViewModel viewModel);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
