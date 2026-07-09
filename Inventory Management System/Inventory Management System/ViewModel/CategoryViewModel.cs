using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModel
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
