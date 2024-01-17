using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Api.Dtos.Stock;
using Api.Helpers;
using Api.Interfaces;
using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(stock => stock.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(comment => comment.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(stock => stock.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(stock => stock.Symbol.Contains(query.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDesc
                        ? stocks.OrderByDescending(sort => sort.Symbol)
                        : stocks.OrderBy(sort => sort.Symbol);
                }
            }

            var skipNUmber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNUmber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks
                .Include(comment => comment.Comments)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(stock => stock.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var exisitingStock = await _context.Stocks.FirstOrDefaultAsync(stock => stock.Id == id);

            if (exisitingStock == null)
            {
                return null;
            }

            exisitingStock.Symbol = stockDto.Symbol;
            exisitingStock.CompanyName = stockDto.CompanyName;
            exisitingStock.Purchase = stockDto.Purchase;
            exisitingStock.LastDiv = stockDto.LastDiv;
            exisitingStock.Industry = stockDto.Industry;
            exisitingStock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();

            return exisitingStock;
        }
    }
}
