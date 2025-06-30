using Allup_Service.Abstractions.Dtos;
using Allup_Service.Dtos.OrderItemDtos;
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.OrderDtos
{
    public class OrderCreateDto:IDto
    {
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Name Duzgun daxil edin.")]
        public string? Name { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "SurName Duzgun daxil edin.")]
        public string? Surname { get; set; } 
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "City Duzgun daxil edin.")]
        public string City { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Apartment Duzgun daxil edin.")]
        public string? Apartment { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "CompanyName Duzgun daxil edin.")]
        public string? CompanyName { get; set; }
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Street Duzgun daxil edin.")]
        public string Street { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Country Duzgun daxil edin.")]
        public string? Country { get; set; } = null!;
        [Required]
        [StringLength(maximumLength: 150)]
        [RegularExpression(@"^[^\d]*$", ErrorMessage = "Town Duzgun daxil edin.")]
        public string? Town { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;

        public List<OrderItemCreateDto> OrderItems { get; set; } = [];
        //public string stripeToken { get; set; } = null!;
        //public string stripeEmail { get; set; } = null!;
    }
}
