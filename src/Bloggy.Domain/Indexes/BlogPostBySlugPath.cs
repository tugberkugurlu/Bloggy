using System.Linq;
using Bloggy.Domain.Entities;
using Raven.Client.Indexes;

namespace Bloggy.Domain.Indexes
{
    public class BlogPostBySlugPath : AbstractIndexCreationTask<BlogPost>
    {
        public class Result
        {
            public string[] SlugPathes { get; set; }
        }

        public BlogPostBySlugPath()
        {
            Map = blogPosts => from blogPost in blogPosts
                               where blogPost.IsApproved
                               select new
                               {
                                   SlugPathes = blogPost.Slugs.Select(slug => slug.Path)
                               };
        }
    }
}