using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Core.Comman
{
    public class BaseAuditableEntity:BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public string CreatedBy { get; set; } = null!;
        [Required]
        public string UpdatedBy { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public bool IsActivated { get; set; }
    }
}
