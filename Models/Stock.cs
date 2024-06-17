namespace Inventory.Models
{
    public class Stock
    {
        public int StockID { get; set; }
        public required string StockCode { get; set; }
        public required string ProductCode { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
