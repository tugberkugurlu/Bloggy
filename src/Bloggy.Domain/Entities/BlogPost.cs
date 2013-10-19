using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Slug> Slugs { get; set; }

        public bool AllowComments { get; set; }
        public bool IsApproved { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public string CreationIp { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
        public string LastUpdateIp { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    public class Slug
    {
        public string Path { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}