using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
