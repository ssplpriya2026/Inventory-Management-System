using Inventory_Management_System.Models;
using Inventory_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inventory_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IStockTransactionService _stockTransactionService;

        public HomeController(IProductService productService, IStockTransactionService stockTransactionService)
        {
            _productService = productService;
            _stockTransactionService = stockTransactionService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = await _productService.GetDashBoardAsync();
            var recentTransaction = await _stockTransactionService.GetRecentTransactionAsync();

            viewModel.RecentTransaction = recentTransaction;
            return View(viewModel);
        }

    }
}
