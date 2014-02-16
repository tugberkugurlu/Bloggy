using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bloggy.Client.Web.Infrastructure.AtomPub.Models;
using System.Web.Http;
using Bloggy.Domain.Entities;
using Raven.Client;
using System.Configuration;

namespace Bloggy.Client.Web.Controllers.AtomPub
{
    public class PostsController : RavenApiController
    {
        public PostsController(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        // GET api/posts
        public async Task<PostFeed> Get()
        {
            IEnumerable<BlogPost> blogPosts = await RavenSession.Query<BlogPost>()
                .Take(int.MaxValue)
                .ToListAsync();

            PostFeed feed = new PostFeed
            {
                Title = ConfigurationManager.AppSettings["Bloggy:Title"],
                Author = ConfigurationManager.AppSettings["Bloggy:Author"],
                Summary = ConfigurationManager.AppSettings["Bloggy:MainDescription"],
                Posts = blogPosts.Select(p =>
                    new PostModel(p, GetCategoryScheme())).OrderByDescending(p => p.PublishDate).ToArray()
            };

            return feed;
        }

        // GET api/posts/5
        public async Task<PostModel> Get(int id)
        {
            BlogPost blogPost = await RavenSession.LoadAsync<BlogPost>(id);
            if (blogPost == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return new PostModel(blogPost, GetCategoryScheme());
        }

        // DELETE api/posts/5
        public async Task<HttpResponseMessage> Delete(int id)
        {
            BlogPost blogPost = await RavenSession.LoadAsync<BlogPost>(id);
            if (blogPost == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            RavenSession.Delete(blogPost);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        // privates

        private string GetCategoryScheme()
        {
            return Url.Link("DefaultApi", new { controller = "tags" });
        }
    }
}