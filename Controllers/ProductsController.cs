using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Models;
using Microsoft.Data.SqlClient;
using Inventory.Utils;

namespace Inventory.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                // son eklenene göre sırala
                .OrderByDescending(m => m.ProductID)
                .ToListAsync()
             );
        }

        // Silme
        public async Task<IActionResult> Delete(int id)
        {
            if (!Helper.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt varsa sil
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            // Listeye yönlendir
            return RedirectToAction(nameof(Index));
        }

        // Oluşturma formu
        public IActionResult Create()
        {
            return View();
        }

        // Oluşturma işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductCode,Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    // Kayıt başarılıysa listeye yönlendir
                    return RedirectToAction(nameof(Index));
                }
                // Kayıt hatalarını yakala
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlException)
                    {
                        // Duplicate Primary Key
                        if (sqlException.Number == 2627 || sqlException.Number == 2601)
                        {
                            product.ProductCode = "";
                            ModelState.AddModelError("ProductCode", "Aynı ürün koduna sahip bir ürün zaten mevcut!");
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
            return View(product);
        }

        // Düzenleme formu
        public async Task<IActionResult> Edit(int id)
        {
            if (!Helper.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt mevcut değilse hata göster
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductCode,Name,Description,Price")] Product product)
        {
            // GET ve POST ID eşleşmeli
            if (id != product.ProductID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    // Kayıt başarılıysa flash mesajı göster
                    TempData["Status"] = "success";
                    TempData["Message"] = "Ürün güncellendi.";
                }
                // Kayıt hatalarını yakala
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlException)
                    {
                        // Ürün kodu değiştiyse ve yeni ürün kodu zaten farklı bir ürüne aitse hata göster
                        if (sqlException.Number == 2627 || sqlException.Number == 2601)
                        {
                            product.ProductCode = "";
                            ModelState.AddModelError("ProductCode", "Yeni ürün koduna sahip bir ürün zaten mevcut!");
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

            return View(product);
        }
    }

}
