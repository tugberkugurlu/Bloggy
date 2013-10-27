using Bloggy.Client.Web.Infrastructure.AtomPub;
using System.Collections.Generic;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Models
{
    public class PostFeed : Resource, IPublicationFeed
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public IEnumerable<PostModel> Posts { get; set; }

        IEnumerable<IPublication> IPublicationFeed.Items
        {

            get { return Posts; }
        }
    }
}