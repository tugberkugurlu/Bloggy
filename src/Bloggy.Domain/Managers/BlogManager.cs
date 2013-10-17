using Bloggy.Domain.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggy.Domain.Managers
{
    public class BlogManager : IBlogManager
    {
        private readonly IAsyncDocumentSession _documentSession;

        public BlogManager(IAsyncDocumentSession documentSession)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            _documentSession = documentSession;
        }

        public Task<BlogPost> GetBlogPostAsync(string id)
        {
            return _documentSession.LoadAsync<BlogPost>(id);
        }

        public async Task<BlogPost> GetBlogPostBySlugAsync(string slug)
        {
            IEnumerable<BlogPost> blogPosts = await _documentSession.Query<BlogPost>()
                .Where(post => post.Slugs.Any(slugEntity => slugEntity.Path == slug))
                .Take(1)
                .ToListAsync();

            return blogPosts.FirstOrDefault();
        }

        public Task AddCommentAsync(string blogPostId, BlogPostComment blogPostComment)
        {
            if (blogPostComment == null)
            {
                throw new ArgumentNullException("blogPostComment");
            }

            blogPostComment.BlogPostId = blogPostId;
            blogPostComment.CreatedOn = DateTimeOffset.Now;
            blogPostComment.LastUpdatedOn = DateTimeOffset.Now;

            return _documentSession.StoreAsync(blogPostComment);
        }
    }
}
