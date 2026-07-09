using Inventory_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class StockTransactionViewModel
    {
        [Required(ErrorMessage = "You have to select one product.")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please select a transaction type.")]
        public TransactionType Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [StringLength(200)]
        public string? Note { get; set; }
        public List<Product>? Products { get; set; }
    }
}
