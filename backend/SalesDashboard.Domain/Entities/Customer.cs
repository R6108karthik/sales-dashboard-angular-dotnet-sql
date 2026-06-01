namespace SalesDashboard.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}