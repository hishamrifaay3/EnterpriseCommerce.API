using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
