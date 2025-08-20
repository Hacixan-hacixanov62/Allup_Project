using Allup_Core.Comman;

namespace Allup_Core.Entities
{
    public class Chat:BaseAuditableEntity
    {
        public string? Name { get; set; }
        public List<Message> Messages { get; set; } = [];
        public List<AppUserChat> AppUserChats { get; set; } = [];
        

    }
}
