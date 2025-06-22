
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.ColorDtos
{
    public class ColorGetDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
