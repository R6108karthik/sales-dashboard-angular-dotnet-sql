using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SalesDashboard.Application.DTOs;
using SalesDashboard.Application.Messaging;
using SalesDashboard.Domain.Entities;
using SalesDashboard.Infrastructure.Data;

namespace SalesDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly SalesDashboardDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<OrdersController> _logger;
    private const string DashboardCacheKey = "dashboard-summary";

    public OrdersController(SalesDashboardDbContext context, IDistributedCache cache, IMessagePublisher messagePublisher, ILogger<OrdersController> logger)
    {
        _context = context;
        _cache = cache;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product)
            .OrderByDescending(order => order.Id)
            .Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.CustomerName,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(order => order.Customer)
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product)
            .Where(order => order.Id == id)
            .Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.CustomerName,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto request)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest("Order must contain at least one product.");
        }

        var customerExists = await _context.Customers
            .AnyAsync(customer => customer.Id == request.CustomerId);

        if (!customerExists)
        {
            return BadRequest("Invalid customer.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Status = "Completed",
            OrderDate = DateTime.UtcNow
        };

        foreach (var requestItem in request.Items)
        {
            if (requestItem.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == requestItem.ProductId);

            if (product is null)
            {
                return BadRequest($"Invalid product id: {requestItem.ProductId}");
            }

            if (product.StockQuantity < requestItem.Quantity)
            {
                return BadRequest($"Not enough stock for product: {product.ProductName}");
            }

            var totalPrice = product.Price * requestItem.Quantity;

            product.StockQuantity -= requestItem.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = requestItem.Quantity,
                UnitPrice = product.Price,
                TotalPrice = totalPrice
            });

            order.TotalAmount += totalPrice;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        try
        {
            await _cache.RemoveAsync(DashboardCacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis is unavailable. Dashboard cache was not cleared.");
        }

        await _messagePublisher.PublicAsync(new OrderCreatedMessage 
        { 
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = DateTime.UtcNow
       });
 
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, null);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders
            .Include(order => order.OrderItems)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        try
        {
            await _cache.RemoveAsync(DashboardCacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis is unavailable. Dashboard cache was not cleared.");
        }

        return NoContent();
    }
}