namespace QuickCommerce.Core.DTOs.Search
{
    public class GlobalSearchItemDto
    {
        public string Type { get; set; } = "";
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Route { get; set; }
    }
}