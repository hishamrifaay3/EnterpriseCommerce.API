using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public class WebsiteReview
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public int Rating { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
