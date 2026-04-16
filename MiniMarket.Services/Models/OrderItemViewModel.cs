using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMarket.Services.Models;

public class OrderItemViewModel
{
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
}