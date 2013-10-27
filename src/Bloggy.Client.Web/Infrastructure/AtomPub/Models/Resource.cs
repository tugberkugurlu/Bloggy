using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System.Collections.Generic;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Models
{
    public class Resource
    {
        private readonly List<Link> links;

        public IEnumerable<Link> Links { get { return links; } }

        public Resource()
        {
            links = new List<Link>();
        }

        public void AddLink(Link link)
        {
            links.Add(link);
        }
    }
}