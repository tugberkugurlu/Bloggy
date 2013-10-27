using System;
using System.Collections.Generic;

namespace Bloggy.Client.Web.Models
{
    public class BlogPostModelLight
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string BriefInfo { get; set; }
        public string Content { get; set; }
        public List<TagModel> Tags { get; set; }
        public string Slug { get; set; }
        public bool AllowComments { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        public BlogPostModelLight()
        {
            Tags = new List<TagModel>();
        }
    }
}