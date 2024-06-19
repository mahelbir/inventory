using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class Stock
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockID { get; set; }

        [Key]
        [Required(ErrorMessage = "Stok Kodu gereklidir.")]
        [StringLength(50, ErrorMessage = "Stok Kodu en fazla 50 karakter olabilir.")]
        public string StockCode { get; set; }

        [Required(ErrorMessage = "Ürün Kodu gereklidir.")]
        [StringLength(50, ErrorMessage = "Ürün Kodu en fazla 50 karakter olabilir.")]
        public string ProductCode { get; set; }

        [ForeignKey("ProductCode")]
        public virtual Product? Product { get; set; }

        [Required(ErrorMessage = "Miktar gereklidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar geçersiz.")]
        public int Quantity { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
