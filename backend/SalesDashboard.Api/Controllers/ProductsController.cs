using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesDashboard.Application.DTOs;
using SalesDashboard.Domain.Entities;
using SalesDashboard.Infrastructure.Data;

namespace SalesDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SalesDashboardDbContext _context;

    public ProductsController(SalesDashboardDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await _context.Products
            .OrderByDescending(product => product.Id)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products
            .Where(product => product.Id == id)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            })
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto request)
    {
        var product = new Product
        {
            ProductName = request.ProductName,
            Category = request.Category,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = new ProductDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            Category = product.Category,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, CreateProductDto request)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        product.ProductName = request.ProductName;
        product.Category = request.Category;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}