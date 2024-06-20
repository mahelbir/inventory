using Inventory.Models;

namespace Inventory.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
