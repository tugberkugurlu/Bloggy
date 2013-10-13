using System;

namespace Bloggy.Client.Web.Models
{
    public class BlogPostModelLight
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BriefInfo { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public string ActiveUrlPath { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        public LanguageModel Language { get; set; }
        public AuthorModel Author { get; set; }
    }
}