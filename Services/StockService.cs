using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Services
{
    public class StockService(ApplicationDbContext context)
    {
        public async Task<List<Stock>> GetAllByProductCode(string productCode)
        {
            var stocks = await context.Stocks
                 // ilgili ürün
                 .Where(s => s.ProductCode == productCode)
                 // son güncellemeye göre listele
                 .OrderByDescending(m => m.UpdatedAt)
                 .ToListAsync();

            return stocks;
        }

        public async Task<Stock?> GetByID(int id)
        {
            return await context.Stocks.FirstOrDefaultAsync(p => p.StockID == id);
        }

        public async Task<Stock?> Delete(int id)
        {
            var stock = await GetByID(id);
            if (stock != null)
            {
                context.Stocks.Remove(stock);
                await context.SaveChangesAsync();
            }

            return stock;
        }

        public async Task Add(Stock stock)
        {
            stock.CreatedAt = DateTime.Now;
            stock.UpdatedAt = DateTime.Now;
            context.Add(stock);
            await context.SaveChangesAsync();
        }

        public async Task Update(Stock stock)
        {
            var existingStock = await GetByID(stock.StockID);
            if (existingStock == null)
            {
                return;
            }

            // Güncellenebilir alanlar
            existingStock.Quantity = stock.Quantity;
            existingStock.CreatedAt = existingStock.CreatedAt;
            existingStock.UpdatedAt = DateTime.Now;

            context.Update(existingStock);

            // Güncellenemez alanlar
            context.Entry(existingStock).Property(s => s.StockID).IsModified = false;
            context.Entry(existingStock).Property(s => s.StockCode).IsModified = false;
            context.Entry(existingStock).Property(s => s.ProductCode).IsModified = false;

            await context.SaveChangesAsync();
        }

    }
}
