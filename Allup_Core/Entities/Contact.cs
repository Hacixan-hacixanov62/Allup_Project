using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class Contact:BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
