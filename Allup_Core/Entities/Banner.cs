using Allup_Core.Attributes;
using Allup_Core.Comman;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Core.Entities
{
    public class Banner:BaseEntity
    {
        public bool IsActivated { get; set; }
        [Required]
        [StringLength(200)]
        public string RedirectUrl { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public string Image { get; set; }
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile? ImageFile { get; set; }
    }
}
