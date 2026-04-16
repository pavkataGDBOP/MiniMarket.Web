using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace MiniMarket.Data.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public string? ImageUrl { get; set; }
}
