
namespace Bloggy.Client.Web
{
    public static class NumericExtensions
    {
        public static int ToIntId(this string id)
        {
            return int.Parse(id.Substring(id.LastIndexOf('/') + 1));
        }
    }
}