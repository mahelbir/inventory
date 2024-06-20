using Microsoft.EntityFrameworkCore;
using Inventory.Models;
using Inventory.Data;

namespace Inventory.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByID(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<Product?> Delete(int id)
        {
            var product = await GetByID(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return product;
        }

        public async Task Add(Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Product product)
        {
            var existingProduct = await GetByID(product.ProductID);
            if (existingProduct == null)
            {
                return;
            }

            // Güncellenebilir alanlar
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;

            _context.Update(existingProduct);

            // Güncellenemez alanlar
            _context.Entry(existingProduct).Property(p => p.ProductID).IsModified = false;
            _context.Entry(existingProduct).Property(p => p.ProductCode).IsModified = false;

            await _context.SaveChangesAsync();
        }

        public async Task<int> Count()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<int> GetTotalStockQuantity(string productCode)
        {
            var product = await _context.Products
                .Include(p => p.Stocks) // Stokları da dahil et
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);

            if (product == null || product.Stocks == null)
            {
                return 0;
            }

            return product.Stocks.Sum(s => s.Quantity);
        }

        public async Task<(List<Product> Products, int TotalCount)> Search(string searchTerm, int pageIndex, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) || // Name LIKE %searchTerm%
                    p.ProductCode == searchTerm // ProductCode = searchTerm
                );
            }

            var totalCount = await query.CountAsync();
            var products = await query
                .OrderByDescending(m => m.ProductID) // En son eklenme sırasına göre sırala
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

    }
}
