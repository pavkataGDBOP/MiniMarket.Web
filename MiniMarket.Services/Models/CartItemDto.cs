using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMarket.Services.Models;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}