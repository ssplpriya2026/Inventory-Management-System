using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public enum TransactionType
    {
        IN,
        OUT
    }
    public class StockTransaction
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public  string Note { get; set; }
    }
}
