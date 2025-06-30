using Allup_Service.Dtos.OrderItemDtos;

namespace Allup_Service.Dtos.OrderDtos
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Apartment { get; set; }
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
        public string? Town { get; set; }
        public string Street { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
       public List<OrderItemUpdateDto> OrderItems { get; set; } = [];
    }
}
