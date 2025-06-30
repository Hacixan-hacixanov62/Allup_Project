
using Allup_Core.Comman;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Core.Entities
{
    public class OrderItem:BaseAuditableEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Count { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}
