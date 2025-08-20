using Allup_Core.Attributes;
using Allup_Core.Comman;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup_Core.Entities
{
    public class About : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        [NotMapped]
        [MaxSize(2 * 1024 * 1024)]
        [AllowedTypes("image/jpeg", "image/png")]
        public IFormFile ImageFile { get; set; } = null!;
        public string Title1 { get; set; } = null!;
        public string Title2 { get; set; } = null!;
        public string Title3 { get; set; } = null!;
        public string Desc1 { get; set; } = null!;
        public string Desc2 { get; set; } = null!;
        public string Desc3 { get; set; } = null!;


    }
}
