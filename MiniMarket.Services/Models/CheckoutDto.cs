using System.ComponentModel.DataAnnotations;

namespace MiniMarket.Services.Models;

using System.ComponentModel.DataAnnotations;


public class CheckoutDto
{
    [Required(ErrorMessage = "First name is required")]
    [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Only letters allowed")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Only letters allowed")]
    public string LastName { get; set; } = null!;


    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public string PaymentMethod { get; set; } = null!;

    public string? CardNumber { get; set; }
    public string? CardHolder { get; set; }
    public string? Expiry { get; set; }
    public string? CVV { get; set; }
}