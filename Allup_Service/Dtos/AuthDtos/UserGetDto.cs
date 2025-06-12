using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.AuthDtos
{
    public class UserGetDto:IDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
