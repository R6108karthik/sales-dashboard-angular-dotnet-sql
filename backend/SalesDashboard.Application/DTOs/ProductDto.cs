namespace SalesDashboard.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
}