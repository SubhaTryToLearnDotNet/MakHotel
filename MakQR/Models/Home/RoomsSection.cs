namespace MakQR.Models.Home
{
    public class RoomsSection
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public List<RoomGridItem> Grid { get; set; } = new();
        public DateTime UpdatedOn { get; set; }

        public class RoomGridItem
        {
            public string Type { get; set; } = string.Empty;

            // Image
            public string? Image { get; set; }
            public string? Alt { get; set; }

            // Text
            public string? Title { get; set; }
            public string? Description { get; set; }
        }
    }
}
