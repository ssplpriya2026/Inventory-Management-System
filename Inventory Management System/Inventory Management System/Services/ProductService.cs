using Inventory_Management_System.Models;
using Inventory_Management_System.UoW;
using Inventory_Management_System.ViewModel;

namespace Inventory_Management_System.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // get all
        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var viewModels = new List<ProductViewModel>();

            foreach(var product in products)
            {
                var viewModel = await ToProductViewModelAsync(product);
                viewModels.Add(viewModel);
            }
            return viewModels;
        }
        // get by id
        public async Task<ProductViewModel> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return await ToProductViewModelAsync(product);
        }

        // get Lowstock list
        public async Task<List<ProductViewModel>> GetLowStockAsync()
        {
            var products = await _unitOfWork.Products.GetLowStockAsync();
            var viewModels = new List<ProductViewModel>();

            foreach (var product in products)
            {
                var viewModel = await ToProductViewModelAsync(product);
                viewModels.Add(viewModel);
            }
            return viewModels;
        }
        
        // get category dropdown
        public async Task<ProductViewModel> GetCreateFormAsync()
        {
            var viewModel = new ProductViewModel();
            viewModel.Categories = await GetCategoryListAsync();
            return viewModel;
        }
        // edit category dropdown
        public async Task<ProductViewModel> GetEditFormAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            var viewModel = await ToProductViewModelAsync(product);
            viewModel.Categories = await GetCategoryListAsync();
            return viewModel;
        }
        // add new product
        public async Task<ServiceResult> CreateAsync(ProductViewModel viewModel)
        {
            var skuExists = await _unitOfWork.Products.SkuExistsAsync(viewModel.SKU, null);
            if (skuExists)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = $"SKU '{viewModel.SKU}' is already in use by another product."
                };
            }

            var product = new Product
            {
                Name = viewModel.Name,
                SKU = viewModel.SKU,
                CategoryId = viewModel.CategoryId,
                UnitPrice = viewModel.UnitPrice,
                CurrentStockQuantity = viewModel.CurrentStockQuantity,
                ReorderThreshold = viewModel.ReorderThreshold
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult { Success = true };
        }
        // update product
        public async Task<ServiceResult> UpdateAsync(ProductViewModel viewModel)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(viewModel.Id);

            if (product == null)
            {
                return new ServiceResult { Success = false, ErrorMessage = "Product not found." };
            }

            var skuExists = await _unitOfWork.Products.SkuExistsAsync(viewModel.SKU, viewModel.Id);
            if (skuExists)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = $"SKU '{viewModel.SKU}' is already in use by another product."
                };
            }

            product.Name = viewModel.Name;
            product.SKU = viewModel.SKU;
            product.CategoryId = viewModel.CategoryId;
            product.UnitPrice = viewModel.UnitPrice;
            product.CurrentStockQuantity = viewModel.CurrentStockQuantity;
            product.ReorderThreshold = viewModel.ReorderThreshold;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResult { Success = true };
        }

        // delete product
        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return new ServiceResult { Success = false, ErrorMessage = "Product not found." };
            }

            try
            {
                _unitOfWork.Products.Remove(product);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = "Cannot delete this product because it has stock transaction history."
                };
            }

            return new ServiceResult { Success = true };
        }

        // product viewmodel properties
        private async Task<ProductViewModel> ToProductViewModelAsync(Product product)
        {
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                UnitPrice = product.UnitPrice,
                CurrentStockQuantity = product.CurrentStockQuantity,
                ReorderThreshold = product.ReorderThreshold
            };

            return viewModel;
        }

        // category dropdown
        private async Task<List<CategoryViewModel>> GetCategoryListAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryViewModels = new List<CategoryViewModel>();

            foreach (var category in categories)
            {
                var categoryViewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name
                };
                categoryViewModels.Add(categoryViewModel);
            }

            return categoryViewModels;
        }
    }
}
