using Inventory_Management_System.Services;
using Inventory_Management_System.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            var products = await _productService.GetLowStockAsync();
            return View(products);
        }
 
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = await _productService.GetCreateFormAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var dropDownData = await _productService.GetCreateFormAsync();
                viewModel.Categories = dropDownData.Categories;
                return View(viewModel);
            }
            var result = await _productService.CreateAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage);

                var dropDownData = await _productService.GetCreateFormAsync();
                viewModel.Categories = dropDownData.Categories;
                return View(viewModel);
            }

            TempData["Success"] = " created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _productService.GetEditFormAsync(id);
            if(viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id , ProductViewModel viewModel)
        {
            if(id != viewModel.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                var dropdownData = await _productService.GetEditFormAsync(id);
                viewModel.Categories = dropdownData.Categories;
                return View(viewModel);
            }

            var result = await _productService.UpdateAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                var dropdownData = await _productService.GetEditFormAsync(id);
                viewModel.Categories = dropdownData.Categories;
                return View(viewModel);
            }
            TempData["Success"] = "updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _productService.GetByIdAsync(id);
            if(viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfiremd(int id)
        {
            var result = await _productService.DeleteAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
