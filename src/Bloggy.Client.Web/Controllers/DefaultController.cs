using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain.Entities;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class DefaultController : RavenController
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IMappingEngine _mapper;
        private const int DefaultPageSize = 5;

        public DefaultController(IMvcLogger logger, IAsyncDocumentSession documentSession, IMappingEngine mapper)
            : base(logger)
        {
            _documentSession = documentSession;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                Logger.Error(string.Format("The page number is smaller than 1. Value: {0}", page));
                return HttpNotFound();
            }

            RavenQueryStatistics stats;
            IEnumerable<BlogPost> blogPosts = await _documentSession.Query<BlogPost>()
                .Statistics(out stats)
                .Where(post => post.IsApproved == true)
                .OrderByDescending(post => post.CreatedOn)
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .ToListAsync();

            return View(new HomeViewModel
            {
                BlogPosts = _mapper.Map<IEnumerable<BlogPost>, IEnumerable<BlogPostModelLight>>(blogPosts)
            });
        }
    }
}