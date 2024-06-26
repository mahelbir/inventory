﻿using Microsoft.EntityFrameworkCore;
using Inventory.Models;
using Inventory.Data;

namespace Inventory.Services
{
    public class StockService
    {
        private readonly ApplicationDbContext _context;

        public StockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetById(int id)
        {
            return await _context.Stocks.FirstOrDefaultAsync(p => p.StockID == id);
        }

        public async Task<List<Stock>> GetAllByProductCode(string productCode)
        {
            var stocks = await _context.Stocks
                 // ilgili ürün
                 .Where(s => s.ProductCode == productCode)
                 // miktara göre artan sıralama
                 .OrderBy(m => m.Quantity)
                 .ToListAsync();

            return stocks;
        }

        public async Task<Stock?> DeleteById(int id)
        {
            var stock = await GetById(id);
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
            var existingStock = await GetById(stock.StockID);
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
