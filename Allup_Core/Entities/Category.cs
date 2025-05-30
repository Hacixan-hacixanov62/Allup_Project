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
    public class Category:BaseAuditableEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; }
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile ImageFile { get; set; }


    }
}
