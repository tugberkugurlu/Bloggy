using System;
using System.Linq;
using Bloggy.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Bloggy.Domain.Indexes
{
    public class MaterializedBlogPostIndex : AbstractIndexCreationTask<BlogPost, MaterializedBlogPostIndex.ReduceResult>
    {
        public class ReduceResult : ProjectionResult
        {
            public string TagSlugsSearch { get; set; }
        }

        public class ProjectionResult
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string BriefInfo { get; set; }
            public string Content { get; set; }
            public bool AllowComments { get; set; }
            public Tag[] Tags { get; set; }
            public string Slug { get; set; }
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
                                   CreatedOn = blogPost.CreatedOn,
                                   AllowComments = blogPost.AllowComments,
                                   CommentsCount = comments.Count(comment => comment.IsApproved && !comment.IsSpam),
                                   Slug = blogPost.Slugs.OrderByDescending(slugEntity => slugEntity.CreatedOn).First(slugEntity => slugEntity.IsDefault).Path,
                                   TagSlugsSearch = blogPost.Tags.Select(tag => tag.Slug)
                               };

            Store(x => x.Title, FieldStorage.Yes);
            Store(x => x.BriefInfo, FieldStorage.Yes);
            Store(x => x.Content, FieldStorage.Yes);
            Store(x => x.AllowComments, FieldStorage.Yes);
            Store(x => x.Tags, FieldStorage.Yes);
            Store(x => x.Slug, FieldStorage.Yes);
            Store(x => x.CreatedOn, FieldStorage.Yes);
            Store(x => x.CommentsCount, FieldStorage.Yes);

            Sort(x => x.CommentsCount, SortOptions.Int);

            // ref: http://stackoverflow.com/questions/14835172/ravendb-static-index-query-on-child-collection-objects
            // Any field you are going to use .Search() on should be analyzed.
            Index(x => x.TagSlugsSearch, FieldIndexing.Analyzed);
        }
    }
}