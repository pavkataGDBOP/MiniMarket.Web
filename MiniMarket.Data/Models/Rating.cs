using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMarket.Data.Models
{
    public class Rating
    {
        
            public int Id { get; set; }

            public int ProductId { get; set; }

            public string UserId { get; set; } = null!;

            public int Value { get; set; } // 1–5

            public Product Product { get; set; } = null!;
        
    }
}
