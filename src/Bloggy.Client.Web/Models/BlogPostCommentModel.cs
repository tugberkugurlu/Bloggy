using System;

namespace Bloggy.Client.Web.Models
{
    public class BlogPostCommentModel
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string GravatarHash { get; set; }
        public string Content { get; set; }
        public bool IsByAuthor { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}