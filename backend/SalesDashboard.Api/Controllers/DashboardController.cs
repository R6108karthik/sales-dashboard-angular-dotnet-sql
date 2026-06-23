using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesDashboard.Infrastructure.Data;
using SalesDashboard.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SalesDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly SalesDashboardDbContext _context;

        public DashboardController(SalesDashboardDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
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

            return Ok(summary);
        }
    }
}
