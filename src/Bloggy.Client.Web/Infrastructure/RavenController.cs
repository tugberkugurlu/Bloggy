using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bloggy.Client.Web.Infrastructure.Logging;
using System;
using System.Security.Claims;
using System.Web.Mvc;
using Bloggy.Client.Web.Models;
using Bloggy.Domain;
using Bloggy.Domain.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bloggy.Client.Web.Infrastructure
{
    public abstract class RavenController : Controller
    {
        protected const int DefaultPageSize = 5;
        protected readonly IMvcLogger Logger;
        protected readonly IAsyncDocumentSession DocumentSession;

        protected ClaimsIdentity Identity 
        {
            get
            {
                return User.Identity as ClaimsIdentity;
            }
        }

        public RavenController(IMvcLogger logger, IAsyncDocumentSession documentSession)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            Logger = logger;
            DocumentSession = documentSession;
        }

        protected async Task<PaginatedList<BlogPostModelLight>> GetBlogPostsAsync(int page)
        {
            RavenQueryStatistics stats;
            IEnumerable<MaterializedBlogPostIndex.ReduceResult> results = await DocumentSession
                .Query<MaterializedBlogPostIndex.ReduceResult, MaterializedBlogPostIndex>().Statistics(out stats)
                .OrderByDescending(post => post.CreatedOn)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .AsProjection<MaterializedBlogPostIndex.ReduceResult>()
                .ToListAsync();

            IEnumerable<BlogPostModelLight> blogPosts = results.Select(result => ToBlogPostModelLight(result)).ToList();
            return new PaginatedList<BlogPostModelLight>(blogPosts, page, blogPosts.Count(), stats.TotalResults, DefaultPageSize);
        }

        protected BlogPostModelLight ToBlogPostModelLight(MaterializedBlogPostIndex.ProjectionResult result)
        {
            return new BlogPostModelLight
            {
                Id = result.Id,
                Language = "en",
                Title = result.Title,
                Content = result.Content,
                BriefInfo = result.BriefInfo,
                CreatedOn = result.CreatedOn,
                AllowComments = result.AllowComments,
                Slug = result.Slug,
                Tags = result.Tags.Select(tag => new TagModel
                {
                    Name = tag.Name,
                    Slug = tag.Slug
                })
            };
        }
    }
}