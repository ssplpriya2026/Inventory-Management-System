using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "sku is required.")]
        [StringLength(50)]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "unit price needs to be greater than 0.")]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "stock quantity cannot be negative.")]
        [Display(Name = "Current Stock Quantity")]
        public int CurrentStockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Reorder threshold cannot be negative.")]
        public int ReorderThreshold { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
    }
}
