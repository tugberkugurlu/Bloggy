using System.Linq;
using Bloggy.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bloggy.Domain.Indexes
{
    public class BlogPost_ByYearAndMonth : AbstractIndexCreationTask<BlogPost>
    {
        public BlogPost_ByYearAndMonth()
        {
            Map = blogPosts => from blogPost in blogPosts
                               select new
                               {
                                   blogPost.CreatedOn.Month,
                                   blogPost.CreatedOn.Year
                               };

            Sort(x => x.CreatedOn.Month, SortOptions.Int);
            Sort(x => x.CreatedOn.Year, SortOptions.Int);
        }
    }
}