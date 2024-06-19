using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Services
{
    public class ProductService(ApplicationDbContext context)
    {
        public async Task<List<Product>> GetAll()
        {
            return await context.Products
                // son eklenene göre sırala
                .OrderByDescending(m => m.ProductID)
                .ToListAsync();
        }

        public async Task<Product?> GetByID(int id)
        {
            return await context.Products.FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<Product?> Delete(int id)
        {
            var product = await GetByID(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }

            return product;
        }

        public async Task Add(Product product)
        {
            context.Add(product);
            await context.SaveChangesAsync();
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

            context.Update(existingProduct);

            // Güncellenemez alanlar
            context.Entry(existingProduct).Property(p => p.ProductID).IsModified = false;
            context.Entry(existingProduct).Property(p => p.ProductCode).IsModified = false;

            await context.SaveChangesAsync();
        }

        public async Task<int> GetTotalStockQuantity(string productCode)
        {
            var product = await context.Products
                .Include(p => p.Stocks) // Stokları da dahil et
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);

            if (product == null)
            {
                return 0;
            }

            return product.Stocks.Sum(s => (s != null ? s.Quantity : 0));
        }


    }
}
