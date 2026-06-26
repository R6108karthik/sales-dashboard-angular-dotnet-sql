using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using SalesDashboard.Infrastructure.Data;
using SalesDashboard.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace SalesDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly SalesDashboardDbContext _context;
        private readonly IDistributedCache _cache;
        private const string DashboardCacheKey  = "dashboard-summary";

        public DashboardController(SalesDashboardDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var cachedSummary = await _cache.GetStringAsync(DashboardCacheKey);

            if (!string.IsNullOrWhiteSpace(cachedSummary))
            {
                Response.Headers["X-Cache"] = "HIT";
                return Ok(JsonSerializer.Deserialize<DashboardSummaryDto>(cachedSummary));
            }

            var summary = new DashboardSummaryDto
            {
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalSales = await _context.Orders.SumAsync(order =>
                    (decimal?)order.TotalAmount) ?? 0,
                LowStockProducts = await _context.Products.CountAsync(product =>
                    product.StockQuantity <= 5)
            };

            await _cache.SetStringAsync(
                DashboardCacheKey,
                JsonSerializer.Serialize(summary),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                });

            Response.Headers["X-Cache"] = "MISS";
            return Ok(summary);
        }
    }
}
