using MiniMarket.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMarket.Services.Interfaces;

public interface IProductService
{
    Task CreateAsync(CreateProductDto model);
    Task<IEnumerable<ProductViewModel>> GetAllAsync(int page, int pageSize, int? categoryId, string search);
    Task<ProductViewModel?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
    Task<int> GetCountAsync(int? categoryId, string search);

}
   