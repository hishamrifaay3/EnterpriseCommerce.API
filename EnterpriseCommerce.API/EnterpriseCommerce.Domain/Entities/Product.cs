using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public abstract class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }

        public string ProductType { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public bool IsLowStock()
        {
            return StockQuantity <= LowStockThreshold && StockQuantity > 0;
        }
        public bool IsOutOfStock()
        {
            return StockQuantity <= 0;
        }
    }
}
