namespace MakQR.Models.Home
{
    public class RoomsViewSection
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ViewItem> Items { get; set; } = new();
        public DateTime UpdatedOn { get; set; }
    }

    public class ViewItem
    {
        public string Image { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }
}
