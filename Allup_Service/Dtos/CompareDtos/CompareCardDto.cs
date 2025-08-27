using Allup_Core.Entities;

namespace Allup_Service.Dtos.CompareDtos
{
    public class CompareCardDto
    {
        public List<CompareItemCard> Product { get; set; } = new List<CompareItemCard>();
    }
}
