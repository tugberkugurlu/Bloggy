using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Domain.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bloggy.Client.Web.Controllers
{
    public class ArchiveController : RavenController
    {
        public ArchiveController(IMvcLogger logger, IAsyncDocumentSession documentSession) : base(logger, documentSession)
        {
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
    }
}