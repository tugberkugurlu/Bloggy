using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Raven.Client;
using System;
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
        private readonly int _defaultPageSize = 5;

        public TagsController(IMvcLogger logger, IAsyncDocumentSession documentSession, IMappingEngine mapper)
            : base(logger)
        {

            _documentSession = documentSession;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(string slug, int pageNumber = 1)
        {
            if (pageNumber < 1)
            {
                return HttpNotFound("Given page number not found");
            }

            IList<BlogPost> blogPosts = await RetrieveBlogPostsByTagAsync(slug, pageNumber);
            IList<BlogPostModelLight> lightBlogPosts = _mapper.Map<IList<BlogPost>, IList<BlogPostModelLight>>(blogPosts);

            HomeViewModel homeViewModel = new HomeViewModel()
            {
                BlogPosts = null
            };

            return View(homeViewModel);
        }

        private IQueryable<BlogPost> GetPostsByTagRavenQuery(string tagSlug, int pageNumber)
        {
            return _documentSession.Query<BlogPost>()
                            .Where(t => t.IsApproved == true && t.Tags.Any(tag => tag.Slug == tagSlug))
                            .OrderByDescending(t => t.CreatedOn)
                            .Skip((pageNumber - 1) * _defaultPageSize)
                            .Take(_defaultPageSize);
        }

        private async Task<IList<BlogPost>> RetrieveBlogPostsByTagAsync(string tagSlug, int pageNumber)
        {
            IList<BlogPost> blogPosts = await GetPostsByTagRavenQuery(tagSlug, pageNumber).ToListAsync();

            return blogPosts;
        }
    }
}