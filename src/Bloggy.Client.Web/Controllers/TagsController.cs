using System;
using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain;
using Bloggy.Domain.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class TagsController : RavenController
    {
        private readonly IMappingEngine _mapper;

        public TagsController(IMvcLogger logger, IAsyncDocumentSession documentSession, IMappingEngine mapper)
            : base(logger, documentSession)
        {
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(string slug, int page = 1)
        {
            if (page < 1)
            {
                Logger.Error(string.Format("The page number is smaller than 1. Value: {0}", page));
                return HttpNotFound();
            }

            PaginatedList<BlogPostModelLight> blogPosts = await GetBlogPostsByTagAsync(page, slug);
            return View(new TagsHomeViewModel
            {
                BlogPosts = blogPosts
            });
        }

        [ChildActionOnly]
        public ActionResult List()
        {
            IEnumerable<Tags_Count.ReduceResult> tags = AsyncHelper.RunSync(() => DocumentSession
                .Query<Tags_Count.ReduceResult, Tags_Count>()
                .ToListAsync());

            return View(tags.Select(tag => new TagModel
            {
                Name = tag.Name,
                Slug = tag.Slug,
                Count = tag.Count,
                LastSeenAt = tag.LastSeenAt
            }));
        }

        [ChildActionOnly]
        public async Task<ActionResult> Facets()
        {
            // NOTE: Currently, not in use.

            FacetResults facets = await DocumentSession.Query
                <RecentPopularTagsMapOnly.ReduceResult, RecentPopularTagsMapOnly>()
                                        .Where(x => x.LastSeen > DateTime.UtcNow.AddMonths(-5).ToUtcToday())
                                        .ToFacetsAsync("Raven/Facets/Tags");

            return View(facets);
        }

        // privates

        private async Task<PaginatedList<BlogPostModelLight>> GetBlogPostsByTagAsync(int page, string tagSlug)
        {
            RavenQueryStatistics stats;
            IEnumerable<MaterializedBlogPostIndex.ReduceResult> results = await DocumentSession
                .Query<MaterializedBlogPostIndex.ReduceResult, MaterializedBlogPostIndex>().Statistics(out stats)
                .Search(post => post.TagSlugsSearch, tagSlug)
                .OrderByDescending(post => post.CreatedOn)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .AsProjection<MaterializedBlogPostIndex.ReduceResult>()
                .ToListAsync();

            IEnumerable<BlogPostModelLight> blogPosts = results.Select(result => ToBlogPostModelLight(result)).ToList();
            return new PaginatedList<BlogPostModelLight>(blogPosts, page, blogPosts.Count(), stats.TotalResults, DefaultPageSize);
        }
    }
}