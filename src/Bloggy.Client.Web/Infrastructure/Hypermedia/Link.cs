using System.Runtime.Serialization;

namespace Bloggy.Client.Web.Infrastructure.Hypermedia
{
    /// <summary>
    /// A base class for relation links
    /// </summary>
    /// <see href="https://github.com/benfoster/Fabrik.Common/blob/master/src/Fabrik.Common.WebAPI/Links/Link.cs" />
    public class Link
    {
        [DataMember]
        public string Rel { get; private set; }

        [DataMember]
        public string Href { get; private set; }

        [DataMember]
        public string Title { get; private set; }

        public Link(string rel, string href, string title = null)
        {
            Rel = rel;
            Href = href;
            Title = title;
        }

        public override string ToString()
        {
            return Href;
        }
    }
}