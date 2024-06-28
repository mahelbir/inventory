using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Helpers;
using Inventory.Services;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers
{
    [Authorize(Roles = nameof(Roles.User))]
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
            var viewModel = new ProductListViewModel
            {
                PageIndex = pageIndex,
                SearchTerm = searchTerm
            };
            var (products, totalRows) = await _productService.Search(searchTerm, viewModel.PageIndex, viewModel.PageSize);
            viewModel.Products = products;
            viewModel.TotalRows = totalRows;
            return View(viewModel);
        }

        // Silme formu
        public async Task<IActionResult> Delete(int id)
        {
            if (!Common.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt mevcut değilse hata göster
            var product = await _productService.GetWithStocks(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Silme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Common.IsValidID(id))
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
                            product.ProductCode = string.Empty;
                            ModelState.AddModelError("ProductCode", "Aynı ürün koduna sahip bir ürün zaten mevcut!");
                        }
                    }

                    // Bilinmeyen kayıt hatası ise varsayılan mesaj
                    if (ModelState.ErrorCount == 0)
                    {
                        ModelState.AddModelError(string.Empty, "Kayıt esnasında bir hata oluştu!");
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
            if (!Common.IsValidID(id))
            {
                return BadRequest();
            }

            // Kayıt mevcut değilse hata göster
            var product = await _productService.Get(id);
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
                    ModelState.AddModelError(string.Empty, "Kayıt esnasında bir hata oluştu!");
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
