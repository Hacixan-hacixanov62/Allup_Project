using System.ComponentModel.DataAnnotations;

namespace Allup_Project.ViewModels.AuthVm
{
    public class UserLoginVm
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = false;
    }
}
