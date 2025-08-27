namespace Allup_Core.Entities
{
    public class CompareItemCard
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string Color { get; set; } = null!;
        public int Rating { get; set; }
        public int InStock { get; set; }
    }
}
