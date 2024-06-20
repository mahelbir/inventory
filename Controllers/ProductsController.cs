using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Utils;
using Inventory.Services;

namespace Inventory.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Listeleme
        public async Task<IActionResult> Index(string searchTerm, int pageIndex = 1)
        {
            var PageSize = 10; // Her sayfada kaç ürün görünecek
            pageIndex = pageIndex < 1 ? 1 : pageIndex; // Sayfa numarası 1'den küçük olamaz
            var (products, totalCount) = await _productService.Search(searchTerm, pageIndex, PageSize);
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            var viewModel = new ProductListViewModel
            {
                Products = products,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }

        // Silme formu
        public async Task<IActionResult> Delete(int id)
        {
            if (!Helper.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt mevcut değilse hata göster
            var product = await _productService.GetByID(id);
            if (product == null)
            {
                return NotFound();
            }

            // Toplam stok miktarını hesapla
            ViewBag.Stocks = await _productService.GetTotalStockQuantity(product.ProductCode);

            return View(product);
        }

        // Silme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Helper.IsValidID(id))
            {
                return BadRequest();
            }

            await _productService.Delete(id);

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
        public async Task<IActionResult> Create([Bind("ProductCode,Name,Price,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.Add(product);
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
            var product = await _productService.GetByID(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductCode,Name,Price,Description")] Product product)
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
                    await _productService.Update(product);

                    // Kayıt başarılıysa flash mesajı göster
                    TempData["Status"] = "success";
                    TempData["Message"] = "Ürün güncellendi.";
                }
                // Kayıt hatalarını yakala
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Kayıt esnasında bir hata oluştu!");
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
