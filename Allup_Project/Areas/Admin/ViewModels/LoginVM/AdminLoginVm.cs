using System.ComponentModel.DataAnnotations;

namespace Allup_Project.Areas.Admin.ViewModels.LoginVM
{
    public class AdminLoginVm
    {
        [Required(ErrorMessage = "UserName is requred")]
        public string UsreName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Minimum 6 simbol olmalidir")]
        [MaxLength(10)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
