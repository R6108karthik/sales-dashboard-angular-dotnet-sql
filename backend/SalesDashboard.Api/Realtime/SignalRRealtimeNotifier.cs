using Microsoft.AspNetCore.SignalR;
using SalesDashboard.Api.Hubs;
using SalesDashboard.Application.Realtime;

namespace SalesDashboard.Api.Realtime;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<SalesDashboardHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<SalesDashboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task OrderCreatedAsync(int orderId, int customerId, decimal totalAmount, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("OrderCreated", 
            new { 
                OrderId = orderId, 
                CustomerId = customerId, 
                TotalAmount = totalAmount 
            }, 
            cancellationToken);
    }
}
