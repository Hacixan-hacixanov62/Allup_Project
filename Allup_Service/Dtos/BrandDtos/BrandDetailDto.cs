using Allup_Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Dtos.BrandDtos
{
    public class BrandDetailDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [MaxLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
    }
}
