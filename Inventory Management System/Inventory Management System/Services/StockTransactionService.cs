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
                Date = DateTime.Now.Date
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
    }
}
