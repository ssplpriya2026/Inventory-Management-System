using Inventory_Management_System.Models;
using Inventory_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inventory_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = await _productService.GetDashBoardAsync();
            return View(viewModel);
        }

    }
}
