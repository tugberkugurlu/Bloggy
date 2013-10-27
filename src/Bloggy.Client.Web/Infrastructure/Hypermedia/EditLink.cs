
namespace Bloggy.Client.Web.Infrastructure.Hypermedia
{
    public class EditLink : Link
    {
        public const string Relation = "edit";

        public EditLink(string href, string title = null)
            : base(Relation, href, title)
        {
        }
    }
}