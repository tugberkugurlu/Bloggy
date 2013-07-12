using Bloggy.Domain.Entities;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloggy.Domain.Indexes
{
    public class Tags_Count : AbstractIndexCreationTask<BlogPost, Tags_Count.ReduceResult>
    {
        public class ReduceResult
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public DateTimeOffset LastSeenAt { get; set; }
        }

        public Tags_Count()
        {
            Map = blogPosts => from blogPost in blogPosts
                               from tag in blogPost.Tags
                               select new
                               {
                                   Name = tag.ToLowerInvariant(),
                                   Count = 1,
                                   LastSeenAt = blogPost.CreatedOn
                               };

            Reduce = results => from tagCount in results
                                group tagCount by tagCount.Name
                                into groupedResult
                                select new 
                                {
                                    Name = groupedResult.Key,
                                    Count = groupedResult.Sum(x => x.Count),
                                    LastSeenAt = groupedResult.Max(x => x.LastSeenAt)
                                };
        }
    }
}
