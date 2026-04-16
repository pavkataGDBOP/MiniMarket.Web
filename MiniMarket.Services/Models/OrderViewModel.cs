using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MiniMarket.Services.Models;

public class OrderViewModel
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    // 🆕 нови полета
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public List<OrderItemViewModel> Items { get; set; } = new();
}