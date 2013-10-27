
namespace Bloggy.Client.Web.Infrastructure.AtomPub.Models
{
    public class TagModel : IPublicationCategory
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        string IPublicationCategory.Label
        {
            get { return Name; }
        }
    }
}