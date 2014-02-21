using System;

namespace Bloggy.Domain.Entities
{
    public class BlogMedia
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string MediaUrl { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}
