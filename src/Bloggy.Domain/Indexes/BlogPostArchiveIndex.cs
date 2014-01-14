using System.Linq;
using Bloggy.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bloggy.Domain.Indexes
{
    // ref: http://stackoverflow.com/questions/11900478/ravendb-map-reduce-with-grouping-by-date
    public class BlogPostArchiveIndex : AbstractIndexCreationTask<BlogPost, BlogPostArchiveIndex.ArchiveItem>
    {
        public class ArchiveItem
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Count { get; set; }
        }

        public BlogPostArchiveIndex()
        {
            Map = blogPosts => from blogPost in blogPosts
                               select new
                               {
                                   Year = blogPost.CreatedOn.Year,
                                   Month = blogPost.CreatedOn.Month,
                                   Count = 1
                               };

            Reduce = items => from result in items
                              group result by new
                              {
                                  result.Year,
                                  result.Month
                              } into agg
                              select new
                              {
                                  Year = agg.Key.Year,
                                  Month = agg.Key.Month,
                                  Count = agg.Sum(x => x.Count)
                              };

            Sort(x => x.Year, SortOptions.Int);
            Sort(x => x.Month, SortOptions.Int);
            Sort(x => x.Count, SortOptions.Int);
        }
    }
}