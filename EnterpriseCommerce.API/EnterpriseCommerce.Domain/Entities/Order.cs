using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalPrice { get; set; }

        // حالة الطلب (Pending, Processing, Completed, Cancelled)
        public string Status { get; set; } = "Pending";

        // --- نظام الأوفلاين والأمان (Offline Sync Idempotency) ---
        // توكن فريد بيتبعت من جهاز الكاشير وهو أوفلاين عشان يمنع تكرار الفاتورة لو ضغط مرتين لما النت يرجع
        public Guid? ClientSyncToken { get; set; }


        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
