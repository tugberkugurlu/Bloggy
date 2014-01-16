using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain;
using Bloggy.Domain.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bloggy.Client.Web.Controllers
{
    public class ArchiveController : RavenController
    {
        public ArchiveController(IMvcLogger logger, IAsyncDocumentSession documentSession)
            : base(logger, documentSession)
        {
        }

        public async Task<ActionResult> Index(int month, int year, int page = 1)
        {
            // TODO: Move the year and month validation to route level.

            if (month < 1 || month > 12)
            {
                Logger.Error(string.Format("The month value is out of range. Value: {0}", month));
                return HttpNotFound();
            }

            if (year < DateTimeOffset.MinValue.Year || year > DateTimeOffset.MaxValue.Year)
            {
                Logger.Error(string.Format("The year value is out of range. Value: {0}", year));
                return HttpNotFound();
            }

            PaginatedList<BlogPostModelLight> blogPosts = await GetBlogPostsByMonthAndYearAsync(page, month, year);
            return View(new ArchiveHomeViewModel
            {
                BlogPosts = blogPosts
            });
        }


        [ChildActionOnly]
        public ActionResult List()
        {
            IEnumerable<BlogPostArchiveIndex.ArchiveItem> archiveItems = AsyncHelper.RunSync(() => DocumentSession
                .Query<BlogPostArchiveIndex.ArchiveItem, BlogPostArchiveIndex>()
                .OrderByDescending(item => item.Year)
                .ThenByDescending(item => item.Month)
                .ToListAsync());

            return View(archiveItems.Select(item => new ArchiveItemModel
            {
                Count = item.Count,
                Month = item.Month,
                Year = item.Year
            }));
        }

        // privates

        private async Task<PaginatedList<BlogPostModelLight>> GetBlogPostsByMonthAndYearAsync(int page, int month, int year)
        {
            RavenQueryStatistics stats;
            IEnumerable<MaterializedBlogPostIndex.ReduceResult> results = await DocumentSession
                .Query<MaterializedBlogPostIndex.ReduceResult, MaterializedBlogPostIndex>().Statistics(out stats)
                .Where(post => post.Month == month && post.Year == year)
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