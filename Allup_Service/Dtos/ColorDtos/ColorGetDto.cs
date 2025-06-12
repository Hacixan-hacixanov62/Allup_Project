
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.ColorDtos
{
    public class ColorGetDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
