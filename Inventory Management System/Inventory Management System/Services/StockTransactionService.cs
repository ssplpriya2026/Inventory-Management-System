using Inventory_Management_System.Models;
using Inventory_Management_System.UoW;
using Inventory_Management_System.ViewModel;

namespace Inventory_Management_System.Services
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // get list of product in stock (in/out)
        public async Task<StockTransactionViewModel> GetCreateFormAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            var viewModel = new StockTransactionViewModel();
            viewModel.Products = products;

            return viewModel;
        }


        public async Task<ServiceResult> ProcessStockMovementAsync(StockTransactionViewModel viewModel)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(viewModel.ProductId);

            if (product == null)
            {
                return new ServiceResult { 
                    Success = false,
                    ErrorMessage = "Product Not Found" };
            }

            if (viewModel.Type == TransactionType.OUT) 
            {
                if(viewModel.Quantity > product.CurrentStockQuantity)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        ErrorMessage = "Not enough stock available."
                    };
                }
            }

            var transaction = new StockTransaction
            {
                ProductId = viewModel.ProductId,
                Type = viewModel.Type,
                Quantity = viewModel.Quantity,
                Note = viewModel.Note,
                Date = DateTime.Now
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (viewModel.Type == TransactionType.IN)
                {
                    product.CurrentStockQuantity = product.CurrentStockQuantity + viewModel.Quantity;
                }
                else
                {
                    product.CurrentStockQuantity = product.CurrentStockQuantity - viewModel.Quantity;
                }

                _unitOfWork.Products.Update(product);
                await _unitOfWork.StockTransactions.AddAsync(transaction);

                //throw new Exception("Test atomicity");

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new ServiceResult
                {
                    Success = true 
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                return new ServiceResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred while recording the transaction. No changes were saved."
                };
            }
        }
        // Dashboard list of recent stock
        public async Task<List<RecentStockTransactionViewModel>> GetRecentTransactionAsync()
        {
            var allTransactions = await _unitOfWork.StockTransactions.GetAllAsync();
            var allproducts = await _unitOfWork.Products.GetAllAsync();

            var sorted = allTransactions.OrderByDescending(t => t.Date).Take(3);

            var result = new List<RecentStockTransactionViewModel>();

            foreach(var transaction in sorted)
            {
                string productName = "Unknown Product";

                foreach(var products in allproducts)
                {
                    if(products.Id == transaction.ProductId)
                    {
                        productName = products.Name;
                    }
                }

                var row = new RecentStockTransactionViewModel();
                row.ProductName = productName;
                row.Type = transaction.Type.ToString();
                row.Quantity = transaction.Quantity;
                row.Date = transaction.Date;

                result.Add(row);
            }
            return result;
        }
    }
}
