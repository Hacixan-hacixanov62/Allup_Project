
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.TagDtos
{
    public class TagGetDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
