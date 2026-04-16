using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace MiniMarket.Services.Models;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}