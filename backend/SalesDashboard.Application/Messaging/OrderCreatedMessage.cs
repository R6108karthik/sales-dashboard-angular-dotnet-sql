using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDashboard.Application.Messaging
{
   public class OrderCreatedMessage
   {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
   }
}
