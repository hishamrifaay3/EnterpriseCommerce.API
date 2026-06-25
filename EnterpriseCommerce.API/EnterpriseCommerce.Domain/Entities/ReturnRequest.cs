using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Domain.Entities
{
    public class ReturnRequest
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }
        public virtual OrderItem OrderItem { get; set; } = null!;

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public string Reason { get; set; } = string.Empty;


        public string CurrentState { get; set; } = "PendingInspection";

        public string ManagerNotes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}
