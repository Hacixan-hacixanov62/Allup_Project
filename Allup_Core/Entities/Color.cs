using Allup_Core.Comman;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Core.Entities
{
    public class Color:BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
