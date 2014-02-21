using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System;
using System.Collections.Generic;

namespace Bloggy.Client.Web.Infrastructure.AtomPub
{
    public interface IPublicationMedia
    {
        string Id { get; }
        string Title { get; }
        string AuthorName { get; }
        string Summary { get; }
        Uri ImageUrl { get; }
        string ContentType { get; }
        DateTimeOffset LastUpdated { get; }
        IEnumerable<Link> Links { get; }
    }
}