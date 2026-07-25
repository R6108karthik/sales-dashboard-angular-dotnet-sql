using System.Threading.Tasks;
using SalesDashboard.Application.DTOs;

namespace SalesDashboard.Application.Realtime
{
    public interface IRealtimeNotifier
    {
         Task OrderCreatedAsync(int orderId, int customerId,
          decimal totalAmount, CancellationToken cancellationToken = default);
    }
}
