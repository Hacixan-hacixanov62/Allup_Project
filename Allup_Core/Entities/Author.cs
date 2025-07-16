

using Allup_Core.Comman;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Core.Entities
{
    public class Author:BaseAuditableEntity
    {
        [Required]
        [StringLength(maximumLength: 50)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Name Duzgun daxil edin.")]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 50)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "SurName Duzgun daxil edin.")]
        public string Surname { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 500)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Desc Duzgun daxil edin.")]
        public string Description { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 100)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Biographia Duzgun daxil edin.")]
        public string Biographia { get; set; } = null!;
        [Required]
        public string ImageUrl { get; set; } = null!;
        [NotMapped]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "FullName Duzgun daxil edin.")]
        public string Fullname { get => $"{Name} {Surname} "; }
    }
}
