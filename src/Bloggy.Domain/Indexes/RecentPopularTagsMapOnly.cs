using System;
using System.Linq;
using Bloggy.Domain.Entities;
using Raven.Client.Indexes;

namespace Bloggy.Domain.Indexes
{
    public class RecentPopularTagsMapOnly : AbstractIndexCreationTask<BlogPost, RecentPopularTagsMapOnly.ReduceResult>
    {
        public class ReduceResult
        {
            public string Tag { get; set; }
            public string Slug { get; set; }
            public int Count { get; set; }
            public DateTimeOffset LastSeen { get; set; }
        }

        public RecentPopularTagsMapOnly()
        {
            Map = blogPosts => from blogPost in blogPosts
                               from tag in blogPost.Tags
                               select new
                               {
                                   Tag = tag.Name,
                                   Slug = tag.Slug,
                                   LastSeen = blogPost.CreatedOn
                               };
        }
    }
}