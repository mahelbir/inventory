using Inventory.Models;

namespace Inventory.ViewModels
{
    public class StockListViewModel
    {
        public IEnumerable<Stock> Stocks { get; set; }

        public string ProductCode { get; set; }

        public int TotalStockQuantity { get; set; }

    }
}
