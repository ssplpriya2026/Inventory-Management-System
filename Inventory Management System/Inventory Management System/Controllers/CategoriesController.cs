using Inventory_Management_System.Services;
using Inventory_Management_System.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Get all categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var result = await _categoryService.CreateAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(viewModel);
            }
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
