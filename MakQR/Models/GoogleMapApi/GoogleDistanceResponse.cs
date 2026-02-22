using static MakQR.Controllers.HomeController;

namespace MakQR.Models.GoogleMapApi
{
    public class GoogleDistanceResponse
    {
        public Row[] rows { get; set; }
    }

    public class Row
    {
        public Element[] elements { get; set; }
    }

    public class Element
    {
        public Value distance { get; set; }

        public Value duration { get; set; }
    }

    public class Value
    {
        public string text { get; set; }

        public int value { get; set; }
    }
}
