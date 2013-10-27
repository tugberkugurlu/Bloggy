using Bloggy.Domain.Entities;
using Raven.Client.Indexes;
using System.Linq;

namespace Bloggy.Domain.Indexes
{
    public class BlogPostsWithCommentsCountTransformer : AbstractTransformerCreationTask<BlogPostComment>
    {
        public class TransformeredResult
        {
            public BlogPost BlogPost { get; set; }
            public int CommentsCount { get; set; }
        }

        public BlogPostsWithCommentsCountTransformer()
        {
            TransformResults = blogPostComments => from comment in blogPostComments
                                                   where comment.IsSpam == false && comment.IsApproved == true
                                                   group comment by comment.BlogPostId into groupedResult
                                                   select new
                                                   {
                                                       BlogPost = LoadDocument<BlogPost>(groupedResult.Key),
                                                       CommentsCount = groupedResult.Count()
                                                   };
        }
    }
}
