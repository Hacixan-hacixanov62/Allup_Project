using System.ComponentModel.DataAnnotations;

namespace Allup_Project.ViewModels.AuthVm
{
    public class ResetPasswordVM
    {
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
