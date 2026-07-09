using Inventory_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IProductService _productService;

        public ReportsController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> StockValue()
        {
            var report = await _productService.GetStockValueReportAsync();
            return View(report);
        }
    }
}
