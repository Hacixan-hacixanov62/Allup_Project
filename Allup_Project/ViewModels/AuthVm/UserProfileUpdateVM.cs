using System.ComponentModel.DataAnnotations;

namespace Allup_Project.ViewModels.AuthVm
{
    public class UserProfileUpdateVM
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string FullName { get; set; } = null!;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
