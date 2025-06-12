using System.ComponentModel.DataAnnotations;

namespace Allup_Project.ViewModels.AuthVm
{
    public class ForgotPasswordVm
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
