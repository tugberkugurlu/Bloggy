using System;
using System.Collections.Generic;

namespace Bloggy.Client.Web.Models
{
    public class BlogPostModelLight : IModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string BriefInfo { get; set; }
        public string Content { get; set; }
        public IEnumerable<TagModel> Tags { get; set; }
        public string Slug { get; set; }
        public bool AllowComments { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}