

using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.ColorDtos
{
    public class ColorUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
