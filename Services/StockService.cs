using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Services
{
    public class StockService
    {
        private readonly ApplicationDbContext _context;

        public StockService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Stock>> GetAllByProductCode(string productCode)
        {
            var stocks = await _context.Stocks
                 // ilgili ürün
                 .Where(s => s.ProductCode == productCode)
                 // son güncellemeye göre listele
                 .OrderByDescending(m => m.UpdatedAt)
                 .ToListAsync();

            return stocks;
        }

        public async Task<Stock?> GetByID(int id)
        {
            return await _context.Stocks.FirstOrDefaultAsync(p => p.StockID == id);
        }

        public async Task<Stock?> Delete(int id)
        {
            var stock = await GetByID(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }

            return stock;
        }

        public async Task Add(Stock stock)
        {
            stock.CreatedAt = DateTime.Now;
            stock.UpdatedAt = DateTime.Now;
            _context.Add(stock);
            await _context.SaveChangesAsync();
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

            _context.Update(existingStock);

            // Güncellenemez alanlar
            _context.Entry(existingStock).Property(s => s.StockID).IsModified = false;
            _context.Entry(existingStock).Property(s => s.StockCode).IsModified = false;
            _context.Entry(existingStock).Property(s => s.ProductCode).IsModified = false;

            await _context.SaveChangesAsync();
        }

    }
}
