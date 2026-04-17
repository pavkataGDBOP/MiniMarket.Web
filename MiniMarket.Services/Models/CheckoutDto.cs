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


    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = null!;

    [Required]
    public string PaymentMethod { get; set; } = null!;

    public string? CardNumber { get; set; }
    [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$", ErrorMessage = "Only letters allowed")]
    public string? CardHolder { get; set; }

    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Format MM/YY")]
    public string? Expiry { get; set; }

    
    [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be exactly 3 digits")]
    public string? CVV { get; set; }
}