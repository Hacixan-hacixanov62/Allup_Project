using Allup_Core.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Dtos.SliderDtos
{
    public class SliderDetailDto
    {
        public string Title { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public string ImageUrl { get; set; }
        [NotMapped]
        [MaxSizeAttribute(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile Photo { get; set; } = null!;
    }
}
