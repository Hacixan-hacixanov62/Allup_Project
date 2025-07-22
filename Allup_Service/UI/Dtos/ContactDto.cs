using System.ComponentModel.DataAnnotations;

namespace Allup_Service.UI.Dtos
{
    public class ContactDto
    {
        [Required(ErrorMessage = "Name sahəsi boş ola bilməz.")]
        [MaxLength(50)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Name Duzgun daxil edin.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Göndəriləcək e-poçt sahəsi boş ola bilməz.")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt ünvanı daxil edin.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Subject sahəsi boş ola bilməz.")]
        public string Subject { get; set; } = null!;
        [Required(ErrorMessage = "Mövzu sahəsi boş ola bilməz.")]
        [MaxLength(200)]
        public string Message { get; set; } = null!;
    }
}
