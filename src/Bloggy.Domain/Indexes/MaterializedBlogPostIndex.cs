using System;
using System.Linq;
using Bloggy.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bloggy.Domain.Indexes
{
    public class MaterializedBlogPostIndex : AbstractIndexCreationTask<BlogPost, MaterializedBlogPostIndex.ReduceResult>
    {
        public class ReduceResult
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string BriefInfo { get; set; }
            public string Content { get; set; }
            public Tag[] Tags { get; set; }
            public Slug[] Slugs { get; set; }
            public DateTimeOffset CreatedOn { get; set; }

            public int CommentsCount { get; set; }
        }

        public MaterializedBlogPostIndex()
        {
            Map = blogPosts => from blogPost in blogPosts
                               where blogPost.IsApproved
                               let comments = blogPost.CommentIds.Select(x => LoadDocument<BlogPostComment>(x))
                               select new
                               {
                                   Title = blogPost.Title,
                                   BriefInfo = blogPost.BriefInfo,
                                   Content = blogPost.Content,
                                   Tags = blogPost.Tags,
                                   Slugs = blogPost.Slugs,
                                   CreatedOn = blogPost.CreatedOn,
                                   CommentsCount = comments.Count(comment => comment.IsApproved && !comment.IsSpam)
                               };

            Store(x => x.Title, FieldStorage.Yes);
            Store(x => x.BriefInfo, FieldStorage.Yes);
            Store(x => x.Content, FieldStorage.Yes);
            Store(x => x.Tags, FieldStorage.Yes);
            Store(x => x.Slugs, FieldStorage.Yes);
            Store(x => x.CreatedOn, FieldStorage.Yes);
            Store(x => x.CommentsCount, FieldStorage.Yes);

            Sort(x => x.CommentsCount, SortOptions.Int);
        }
    }
}