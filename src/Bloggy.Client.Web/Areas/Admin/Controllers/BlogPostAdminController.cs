using AutoMapper;
using Bloggy.Client.Web.Areas.Admin.Models;
using Bloggy.Client.Web.Areas.Admin.RequestModels;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Domain.Entities;
using Raven.Client;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Areas.Admin.Controllers
{
    public class BlogPostAdminController : RavenController
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IMappingEngine _mapper;

        public BlogPostAdminController(IMvcLogger logger, IMappingEngine mapper, IAsyncDocumentSession documentSession) : base(logger)
        {
            _mapper = mapper;
            _documentSession = documentSession;
        }

        public async Task<ActionResult> Details(int id)
        {
            BlogPost blogPost = await _documentSession.LoadAsync<BlogPost>(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            return View(_mapper.Map<BlogPost, BlogPostModel>(blogPost));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<ActionResult> CreatePost(BlogPostRequestModel requestModel)
        {
            if (ModelState.IsValid)
            {
                BlogPost blogPost = ConstructBlogPost(requestModel);
                await _documentSession.StoreAsync(blogPost);
                await _documentSession.SaveChangesAsync();

                return RedirectToAction("Details", new { id = blogPost.Id.ToIntId() });
            }

            return View(requestModel);
        }

        // private helpers

        private BlogPost ConstructBlogPost(BlogPostRequestModel requestModel)
        {
            BlogPost blogPost = _mapper.Map<BlogPostRequestModel, BlogPost>(requestModel);
            blogPost.CreatedOn = DateTimeOffset.Now;
            blogPost.CreationIp = Request.UserHostAddress;
            blogPost.LastUpdatedOn = blogPost.CreatedOn;
            blogPost.LastUpdateIp = Request.UserHostAddress;

            return blogPost;
        }
    }
}