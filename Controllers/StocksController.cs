using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Models;
using Microsoft.Data.SqlClient;
using Inventory.Utils;

namespace Inventory.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ürüne göre listeleme
        public async Task<IActionResult> Index(string productCode)
        {
            if (!Helper.IsValidCode(productCode))
            {
                return BadRequest();
            }

            var stocks = await _context.Stocks
                // ilgili ürün
                .Where(s => s.ProductCode == productCode)
                // son güncellemeye göre listele
                .OrderByDescending(m => m.UpdatedAt)
                .ToListAsync();

            ViewBag.ProductCode = productCode;
            ViewBag.Total = stocks.Sum(s => s.Quantity);

            return View(stocks);
        }

        // Silme
        public async Task<IActionResult> Delete(int id)
        {
            if (!Helper.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt varsa sil
            var stock = await _context.Stocks.FindAsync(id);
            string productCode = "";
            if (stock != null)
            {
                productCode = stock.ProductCode;
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }

            // Listeye yönlendir
            return RedirectToAction("Index", new { productCode = productCode });
        }

        // Oluşturma formu
        public IActionResult Create(string productCode)
        {
            if (!Helper.IsValidCode(productCode))
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
                    stock.CreatedAt = DateTime.Now;
                    stock.UpdatedAt = DateTime.Now;
                    _context.Add(stock);
                    await _context.SaveChangesAsync();

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
                            ModelState.AddModelError("StockCode", "Aynı stok koduna sahip bir stok zaten mevcut!");
                        }
                        // Foreign key
                        else if (sqlException.Number == 547)
                        {
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
            if (!Helper.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt mevcut değilse hata göster
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // Düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockID,StockCode,ProductCode,Quantity,CreatedAt")] Stock stock)
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

                        stock.UpdatedAt = DateTime.Now;
                        _context.Update(stock);
                        await _context.SaveChangesAsync();

                        // Kayıt başarılıysa flash mesajı göster
                        TempData["Status"] = "success";
                        TempData["Message"] = "Stok güncellendi.";
                    }
                // Kayıt hatalarını yakala
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlException)
                    {
                        // Duplicate Primary Key
                        if (sqlException.Number == 2627 || sqlException.Number == 2601)
                        {
                            ModelState.AddModelError("StockCode", "Yeni stok koduna sahip bir stok zaten mevcut!");
                        }
                        // Foreign key
                        else if (sqlException.Number == 547)
                        {
                            ModelState.AddModelError("ProductCode", "Geçersiz ürün kodu! Lütfen geçerli bir ürün kodu giriniz.");
                        }
                    }

                    // Bilinmeyen kayıt hatası ise varsayılan mesaj
                    if (ModelState.ErrorCount == 0)
                    {
                        ModelState.AddModelError("", "Güncelleme esnasında bir hata oluştu!");
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
