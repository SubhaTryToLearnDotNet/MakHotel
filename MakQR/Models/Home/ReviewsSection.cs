namespace MakQR.Models.Home
{
    public class ReviewsSection
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public int SlideSize { get; set; } = 3;
        public List<ReviewItem> Items { get; set; } = new();
        public DateTime UpdatedOn { get; set; }
    }
    public class ReviewItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CommentHtml { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
    }
}
