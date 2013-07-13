using System;
using System.Collections.Generic;

namespace Bloggy.Domain.Entities
{
    public class BlogPost : ITrackable
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string Language { get; set; }
        public int? SecondaryId { get; set; }

        public string Title { get; set; }
        public string BriefInfo  { get; set; }
        public string Content { get; set; }
        public ICollection<string> Tags { get; set; }
        public string IsApproved { get; set; }
        public bool AllowComments { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public string CreationIp { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
        public string LastUpdateIp { get; set; }

        public ICollection<Slug> Slugs { get; set; }
    }

    public class Slug
    {
        public string Path { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}