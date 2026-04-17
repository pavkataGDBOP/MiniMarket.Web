using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMarket.Data.Models;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;

    public string? CardHash { get; set; }
    public string? CardLast4 { get; set; }
}
