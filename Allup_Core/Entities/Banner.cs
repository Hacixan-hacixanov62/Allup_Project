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

        [StringLength(200)]
        public string? ImageUrl { get; set; }
        [Required]
        [StringLength(200)]
        public string RedirectUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
