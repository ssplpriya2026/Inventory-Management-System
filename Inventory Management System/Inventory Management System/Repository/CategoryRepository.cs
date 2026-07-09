using Inventory_Management_System.Data;
using Inventory_Management_System.Models;

namespace Inventory_Management_System.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
