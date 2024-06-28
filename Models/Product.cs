using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Key]
        [Required(ErrorMessage = "Ürün Kodu gereklidir.")]
        [StringLength(50, ErrorMessage = "Ürün Kodu en fazla 50 karakter olabilir.")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Ürün Adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Ürün Adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir.")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Geçersiz fiyat.")]
        public decimal Price { get; set; }

        [StringLength(255, ErrorMessage = "Açıklama en fazla 255 karakter olabilir.")]
        public string? Description { get; set; }

        public virtual ICollection<Stock>? Stocks { get; set; }

        [NotMapped]
        public int TotalStockQuantity
        {
            get
            {
                if (Stocks == null)
                {
                    throw new InvalidDataException("Stok listesi eksik");
                };
                return Stocks.Sum(s => s.Quantity);
            }
        }
    }
}
