using System.Globalization;

namespace Bloggy.Client.Web.Models
{
    public class ArchiveItemModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }

        public string Label
        {
            get
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
                return string.Format("{0}, {1} ({2})",
                    monthName,
                    Year.ToString(CultureInfo.InvariantCulture),
                    Count.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}