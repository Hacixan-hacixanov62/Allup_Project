

using Allup_Core.Comman;
using System.ComponentModel.DataAnnotations;

namespace Allup_Core.Entities
{
    public class Blog:BaseAuditableEntity
    {
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Title Duzgun daxil edin.")]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 500)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "MinDescription Duzgun daxil edin.")]
        public string MinDescription { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 500)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "MaxDescription Duzgun daxil edin.")]
        public string MaxDescription { get; set; } = null!;
        [Required]
        public string ImageUrl { get; set; } = null!;
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public List<BlogTag> BlogTags { get; set; } = new List<BlogTag>();

    }
}
