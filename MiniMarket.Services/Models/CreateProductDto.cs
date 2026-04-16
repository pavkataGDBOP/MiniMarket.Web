using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiniMarket.Services.Models;

public class CreateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Please select a category")]
    public int? CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }



}