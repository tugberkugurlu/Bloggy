using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bloggy.Domain.Entities
{
    public class BlogPost : IEntity, ITrackable
    {
        public BlogPost()
        {
            Tags = new Collection<Tag>();
            Slugs = new Collection<Slug>();
            CommentIds = new Collection<string>();
        }

        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string Language { get; set; }
        public int? SecondaryId { get; set; }

        public string Title { get; set; }
        public string BriefInfo  { get; set; }
        public string Content { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Slug> Slugs { get; set; }
        public ICollection<string> CommentIds { get; set; }

        public bool AllowComments { get; set; }
        public bool IsApproved { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public string CreationIp { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
        public string LastUpdateIp { get; set; }

        [JsonIgnore]
        public Slug DefaultSlug 
        {
            get
            {
                return Slugs.OrderByDescending(slugEntity => slugEntity.CreatedOn)
                    .FirstOrDefault(slugEntity => slugEntity.IsDefault == true);
            }
        }
    }

    public class Tag
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    public class Slug
    {
        public string Path { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}