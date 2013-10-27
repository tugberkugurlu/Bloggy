
namespace Bloggy.Client.Web.Infrastructure.Hypermedia
{
    public class SelfLink : Link
    {
        public const string Relation = "self";

        public SelfLink(string href, string title = null)
            : base(Relation, href, title)
        {
        }
    }
}