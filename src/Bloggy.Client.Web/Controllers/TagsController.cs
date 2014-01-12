using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class TagsController : RavenController
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IMappingEngine _mapper;
        private const int DefaultPageSize = 5;

        public TagsController(IMvcLogger logger, IAsyncDocumentSession documentSession, IMappingEngine mapper)
            : base(logger)
        {

            _documentSession = documentSession;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(string slug, int page = 1)
        {
            if (page < 1)
            {
                Logger.Error(string.Format("The page number is smaller than 1. Value: {0}", page));
                return HttpNotFound();
            }

            RavenQueryStatistics stats;
            IEnumerable<BlogPost> blogPosts = await _documentSession.Query<BlogPost>()
                .Statistics(out stats)
                .Where(post => post.IsApproved == true && post.Tags.Any(tag => tag.Slug == slug))
                .OrderByDescending(post => post.CreatedOn)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .ToListAsync();

            IEnumerable<BlogPostModelLight> blogPostModels = _mapper.Map<IEnumerable<BlogPost>, IEnumerable<BlogPostModelLight>>(blogPosts).ToArray();
            return View(new TagsHomeViewModel
            {
                BlogPosts = new PaginatedList<BlogPostModelLight>(blogPostModels, page, blogPostModels.Count(), stats.TotalResults)
            });
        }

        [ChildActionOnly]
        public ActionResult List()
        {
            IEnumerable<Tags_Count.ReduceResult> tags = AsyncHelper.RunSync(() => _documentSession
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
    }
}