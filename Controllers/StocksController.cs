using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Models;
using Microsoft.Data.SqlClient;
using Inventory.Helpers;
using Inventory.Services;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers
{
    [Authorize(Roles = nameof(Roles.User))]
    public class StocksController : Controller
    {
        private readonly StockService _stockService;
        private readonly ProductService _productService;

        public StocksController(StockService stockService, ProductService productService)
        {
            _stockService = stockService;
            _productService = productService;
        }

        // Ürüne göre listeleme
        public async Task<IActionResult> Index(string productCode)
        {
            if (!Common.IsValidCode(productCode))
            {
                return BadRequest();
            }

            var product = await _productService.GetByProductCodeWithStocks(productCode);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Silme
        public async Task<IActionResult> Delete(int id)
        {
            if (!Common.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt varsa sil
            var stock = await _stockService.Delete(id);
            var productCode = stock != null ? stock.ProductCode : null;

            // Ürünün stok listesine yönlendir
            return RedirectToAction("Index", new { productCode });
        }

        // Oluşturma formu
        public IActionResult Create(string productCode)
        {
            if (!Common.IsValidCode(productCode))
            {
                return BadRequest();
            }

            var stock = new Stock { ProductCode = productCode, Quantity = 1 };

            return View(stock);
        }

        // Oluşturma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockCode,ProductCode,Quantity")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _stockService.Add(stock);

                    // Kayıt başarılıysa listeye yönlendir
                    return RedirectToAction("Index", new { productCode = stock.ProductCode });
                }
                // Kayıt hatalarını yakala
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlException)
                    {
                        // Duplicate Primary Key
                        if (sqlException.Number == 2627 || sqlException.Number == 2601)
                        {
                            stock.StockCode = "";
                            ModelState.AddModelError("StockCode", "Aynı stok koduna sahip bir stok zaten mevcut!");
                        }
                        // Foreign key
                        else if (sqlException.Number == 547)
                        {
                            stock.ProductCode = "";
                            ModelState.AddModelError("ProductCode", "Geçersiz ürün kodu! Lütfen geçerli bir ürün kodu giriniz.");
                        }
                    }

                    // Bilinmeyen kayıt hatası ise varsayılan mesaj
                    if (ModelState.ErrorCount == 0)
                    {
                        ModelState.AddModelError("", "Kayıt esnasında bir hata oluştu!");
                    }
                }
                // Diğer hatalar
                catch (Exception)
                {
                    return View("Error");
                }

            }

            // Girilen bilgileri kaybetmeden formu göster
            return View(stock);
        }

        // Düzenleme formu
        public async Task<IActionResult> Edit(int id)
        {
            if (!Common.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt mevcut değilse hata göster
            var stock = await _stockService.Get(id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // Düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockID,StockCode,ProductCode,Quantity")] Stock stock)
        {
            // GET ve POST ID eşleşmeli
            if (id != stock.StockID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await _stockService.Update(stock);

                    // Kayıt başarılıysa flash mesajı göster
                    TempData["Status"] = "success";
                    TempData["Message"] = "Stok güncellendi.";
                }
                // Kayıt hatalarını yakala
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlException)
                    {
                        // Foreign key
                        if (sqlException.Number == 547)
                        {
                            stock.ProductCode = "";
                            ModelState.AddModelError("ProductCode", "Geçersiz ürün kodu! Lütfen geçerli bir ürün kodu giriniz.");
                        }
                    }

                    // Bilinmeyen kayıt hatası ise varsayılan mesaj
                    if (ModelState.ErrorCount == 0)
                    {
                        ModelState.AddModelError("", "Kayıt esnasında bir hata oluştu!");
                    }
                }
                // Diğer hatalar
                catch (Exception)
                {
                    return View("Error");
                }
            }

            return View(stock);
        }

    }

}
