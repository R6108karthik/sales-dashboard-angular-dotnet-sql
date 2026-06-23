using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDashboard.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalCustomers { get; set; }

        public int TotalProducts { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalSales { get; set; }

        public int LowStockProducts { get; set; }
    }
}
