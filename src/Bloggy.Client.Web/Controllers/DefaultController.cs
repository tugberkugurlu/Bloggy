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
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class DefaultController : RavenController
    {
        private readonly IMappingEngine _mapper;

        public DefaultController(IMvcLogger logger, IAsyncDocumentSession documentSession, IMappingEngine mapper)
            : base(logger, documentSession)
        {
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(int page = 1)
        {
            if (page < 1)
            {
                Logger.Error(string.Format("The page number is smaller than 1. Value: {0}", page));
                return HttpNotFound();
            }

            PaginatedList<BlogPostModelLight> blogPosts = await GetBlogPostsAsync(page);
            return View(new HomeViewModel
            {
                BlogPosts = blogPosts
            });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}