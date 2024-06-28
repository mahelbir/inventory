using Inventory.Models;

namespace Inventory.ViewModels
{
    public class ProductListViewModel : PageableListViewModel
    {
        public IEnumerable<Product> Products { get; set; }

        public string SearchTerm { get; set; }

    }
}
