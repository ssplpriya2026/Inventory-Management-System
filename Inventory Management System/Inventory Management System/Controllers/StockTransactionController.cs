using Inventory_Management_System.Services;
using Inventory_Management_System.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers
{
    public class StockTransactionController : Controller
    {
        private readonly IStockTransactionService _stockTransactionService;

        public StockTransactionController(IStockTransactionService stockTransactionService)
        {
            _stockTransactionService = stockTransactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = await _stockTransactionService.GetCreateFormAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var dropdownData = await _stockTransactionService.GetCreateFormAsync();
                viewModel.Products = dropdownData.Products;
                return View(viewModel);
            }
            var result = await _stockTransactionService.ProcessStockMovementAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                var dropdownData = await _stockTransactionService.GetCreateFormAsync();
                viewModel.Products = dropdownData.Products;
                return View(viewModel);
            }

            TempData["Success"] = "Stock Movement recorded Successfully.";
            return RedirectToAction(nameof(Create));
        }
    }
}
