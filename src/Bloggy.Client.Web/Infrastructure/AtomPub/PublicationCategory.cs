
namespace Bloggy.Client.Web.Infrastructure.AtomPub
{
    public class PublicationCategory : IPublicationCategory
    {
        public string Name { get; set; }
        public string Label { get; set; }

        public PublicationCategory(string name, string label = null)
        {
            Name = name;
            Label = label;
        }
    }
}