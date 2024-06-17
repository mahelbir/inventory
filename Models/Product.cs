using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Ürün Kodu gereklidir.")]
        [StringLength(50, ErrorMessage = "Ürün Kodu en fazla 50 karakter olabilir.")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Ürün Adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Ürün Adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir.")]
        [Range(0, 999999.99, ErrorMessage = "Fiyat 0 ile 999999.99 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [StringLength(255, ErrorMessage = "Açıklama en fazla 255 karakter olabilir.")]
        public string? Description { get; set; }
    }
}
