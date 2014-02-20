using Bloggy.Client.Web.Infrastructure.AtomPub.Models;
using Bloggy.Domain.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

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

        // POST api/posts
        public async Task<HttpResponseMessage> Post(AddPostCommand command)
        {
            BlogPost post = new BlogPost
            {
                AuthorId = "BlogUsers/1",
                Language = "en-US",
                Title = command.Title,
                BriefInfo = command.Summary,
                Content = command.Content,
                Tags = new Collection<Tag>(command.Tags.Select(tag => new Tag { Name =  tag, Slug = tag.ToSlug() }).ToList()),
                CreatedOn = command.PublishDate ?? DateTimeOffset.Now,
                LastUpdatedOn = command.PublishDate ?? DateTimeOffset.Now,
                AllowComments = true,
                IsApproved = true
            };

            post.Slugs.Add(new Slug 
            {
                IsDefault = true,
                Path = command.Slug ?? command.Title.ToSlug(), 
                CreatedOn = command.PublishDate ?? DateTimeOffset.Now
            });

            await RavenSession.StoreAsync(post);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, new PostModel(post, GetCategoryScheme()));
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { controller = "posts", id = post.Id }));

            return response;
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