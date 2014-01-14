using Bloggy.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Linq;

namespace Bloggy.Domain.Indexes
{
    public class Tags_Count : AbstractIndexCreationTask<BlogPost, Tags_Count.ReduceResult>
    {
        public class ReduceResult
        {
            public string Name { get; set; }
            public string Slug { get; set; }
            public int Count { get; set; }
            public DateTimeOffset LastSeenAt { get; set; }
        }

        public Tags_Count()
        {
            Map = blogPosts => from blogPost in blogPosts
                               from tag in blogPost.Tags
                               select new
                               {
                                   Name = tag.Name.ToLowerInvariant(),
                                   Slug = tag.Slug,
                                   Count = 1,
                                   LastSeenAt = blogPost.CreatedOn
                               };

            Reduce = results => from tagCount in results
                                group tagCount by new { Name = tagCount.Name, Slug = tagCount.Slug }
                                into groupedResult
                                select new 
                                {
                                    Name = groupedResult.Key.Name,
                                    Slug = groupedResult.Key.Slug,
                                    Count = groupedResult.Sum(x => x.Count),
                                    LastSeenAt = groupedResult.Max(x => x.LastSeenAt)
                                };

            Sort(x => x.Count, SortOptions.Int);
        }
    }
}
