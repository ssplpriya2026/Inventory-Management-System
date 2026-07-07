using Inventory_Management_System.Models;
using Inventory_Management_System.UoW;
using Inventory_Management_System.ViewModel;

namespace Inventory_Management_System.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var viewModels = new List<CategoryViewModel>();

            foreach(var category in categories)
            {
                var viewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                };
                viewModels.Add(viewModel);
            }
            return viewModels;
        }

        public async Task<CategoryViewModel?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if(category == null)
            {
                return null;
            }
            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            };
            return viewModel;
        }

        public async Task<ServiceResult> CreateAsync(CategoryViewModel viewModel)
        {
            var category = new Category
            {
                Name = viewModel.Name
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult { Success = true};
        }

        public async Task<ServiceResult> UpdateAsync(CategoryViewModel viewModel)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(viewModel.Id);
            if (category == null)
            {
                return new ServiceResult { Success = false, ErrorMessage = "Category not found." };
            }
            category.Name = viewModel.Name;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult{Success = true};
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return new ServiceResult { Success = false, ErrorMessage = "Category Not Found." };
            }
            try
            {
                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.SaveChangesAsync();
            }
            catch(Exception)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = "Cannot delete this category."
                };
            }
            return new ServiceResult { Success = true };
        }
    }
}
