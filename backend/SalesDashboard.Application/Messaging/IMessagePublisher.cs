using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDashboard.Application.Messaging
{
  public  interface IMessagePublisher
    {
        Task PublicAsync<T>(T message);
    }
}
