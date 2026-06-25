using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public class PhysicalProduct:Product
    {
        public decimal WeightKg { get; set; }

        // أبعاد المنتج الشحن (طول * عرض * ارتفاع)
        public string Dimensions { get; set; } = string.Empty;

        // ميثود تشغيلية خاصة بالمنتجات المادية فقط
        public decimal CalculateShippingCost(decimal ratePerKg)
        {
            return WeightKg * ratePerKg;
        }
    }
}
