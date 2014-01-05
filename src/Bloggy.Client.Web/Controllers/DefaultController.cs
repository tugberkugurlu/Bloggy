using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Raven.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
                return HttpNotFound("Given page number not found");
            }

            IList<BlogPost> blogPosts = await RetrieveBlogPostsAsync(page);
            IList<BlogPostModelLight> lightBlogPosts = _mapper.Map<IList<BlogPost>, IList<BlogPostModelLight>>(blogPosts);

            PagerModel pagerModel = await InitializePagerAsync(page);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                BlogPosts = lightBlogPosts,
                PagerModel = pagerModel
            };

            return View(homeViewModel);
        }

        private async Task<PagerModel> InitializePagerAsync(int pageNumber)
        {
            PagerModel pagerModel = new PagerModel();

            if (pageNumber == 1)
            {
                pagerModel.IsNewerDisabled = true;
                pagerModel.IsOlderDisabled = await GetPostsRavenQuery(pageNumber + 1).AnyAsync();
            }
            else
            {
                pagerModel.IsNewerDisabled = await GetPostsRavenQuery(pageNumber - 1).AnyAsync();
                pagerModel.IsOlderDisabled = await GetPostsRavenQuery(pageNumber + 1).AnyAsync();
            }

            pagerModel.CurrentPage = pageNumber;
            return pagerModel;
        }

        private IQueryable<BlogPost> GetPostsRavenQuery(int pageNumber)
        {
            return _documentSession.Query<BlogPost>()
                            .Where(t => t.IsApproved == true)
                            .OrderByDescending(t => t.CreatedOn)
                            .Skip((pageNumber - 1) * DefaultPageSize)
                            .Take(DefaultPageSize);
        }

        private async Task<IList<BlogPost>> RetrieveBlogPostsAsync(int pageNumber)
        {
            IList<BlogPost> blogPosts = await GetPostsRavenQuery(pageNumber).ToListAsync();

            return blogPosts;
        }
    }
}