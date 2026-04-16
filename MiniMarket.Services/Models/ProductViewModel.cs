using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MiniMarket.Services.Models;

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string Description { get; set; } = null!;
}