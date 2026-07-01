using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SalesDashboard.Application.DTOs;
using SalesDashboard.Domain.Entities;
using SalesDashboard.Infrastructure.Data;

namespace SalesDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly SalesDashboardDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CustomersController> _logger;
    private const string DashboardCacheKey = "dashboard-summary";

    public CustomersController(SalesDashboardDbContext context, IDistributedCache cache, ILogger<CustomersController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetCustomers()
    {
        var customers = await _context.Customers
            .OrderByDescending(customer => customer.Id)
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                City = customer.City
            })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _context.Customers
            .Where(customer => customer.Id == id)
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                City = customer.City
            })
            .FirstOrDefaultAsync();

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto request)
    {
        var customer = new Customer
        {
            CustomerName = request.CustomerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            City = request.City
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        try
        {
            await _cache.RemoveAsync(DashboardCacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis is unavailable. Dashboard cache was not cleared.");
        }

        var response = new CustomerDto
        {
            Id = customer.Id,
            CustomerName = customer.CustomerName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            City = customer.City
        };

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, CreateCustomerDto request)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        customer.CustomerName = request.CustomerName;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;
        customer.City = request.City;

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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
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