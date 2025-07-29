using Allup_Core.Entities;

namespace Allup_Project.ViewModels.AuthVm
{
    public class UserProfileVM
    {
        public UserProfileUpdateVM UserProfileUpdateVM { get; set; } = null!;
        public List<Order> Orders { get; set; } = null!;
        //public List<OrderItem> OrderItems { get; set; } = null!;

    }
}
