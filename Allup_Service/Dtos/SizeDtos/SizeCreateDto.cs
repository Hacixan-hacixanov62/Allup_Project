

using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.SizeDtos
{
    public class SizeCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}
