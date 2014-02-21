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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bloggy.Client.Web.Controllers.AtomPub
{
    [Authorize(Roles = ApplicationRoles.AdminRole)]
    public class PostsController : RavenApiController
    {
        public PostsController(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        // GET api/posts
        public async Task<HttpResponseMessage> Get()
        {
            HttpResponseMessage result;
            ClaimsPrincipal user = User as ClaimsPrincipal;
            Claim userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                IEnumerable<BlogPost> blogPosts = await RavenSession.Query<BlogPost>()
                    .Where(post => post.AuthorId == userIdClaim.Value)
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

                result = Request.CreateResponse(HttpStatusCode.OK, feed);
            }

            return result;
        }

        // GET api/posts/5
        public async Task<HttpResponseMessage> Get(int id)
        {
            HttpResponseMessage result;
            ClaimsPrincipal user = User as ClaimsPrincipal;
            Claim userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                BlogPost blogPost = await RavenSession.LoadAsync<BlogPost>(id);
                if (blogPost == null)
                {
                    result = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (userIdClaim.Value.Equals(blogPost.AuthorId, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        // TODO: Log here
                        // Basically, the blogPost author is not the one who has been authenticated. return 404 for security reasons.
                        result = Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        PostModel post = new PostModel(blogPost, GetCategoryScheme());
                        result = Request.CreateResponse(HttpStatusCode.OK, post);
                    }
                }
            }

            return result;
        }

        // POST api/posts
        public async Task<HttpResponseMessage> Post(AddPostCommand command)
        {
            HttpResponseMessage result;
            ClaimsPrincipal user = User as ClaimsPrincipal;
            Claim userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                BlogPost post = new BlogPost
                {
                    AuthorId = userIdClaim.Value,
                    Language = "en-US",
                    Title = command.Title,
                    BriefInfo = command.Summary,
                    Content = command.Content,
                    Tags = new Collection<Tag>(command.Tags.Select(tag => new Tag { Name = tag, Slug = tag.ToSlug() }).ToList()),
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
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { controller = "posts", id = post.Id.ToIntId() }));
                result = response;
            }

            return result;
        }

        // PUT api/posts/5
        public async Task<HttpResponseMessage> Put(int id, UpdatePostCommand command) 
        {
            HttpResponseMessage result;
            ClaimsPrincipal user = User as ClaimsPrincipal;
            Claim userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                BlogPost blogPost = await RavenSession.LoadAsync<BlogPost>(id);
                if (blogPost == null)
                {
                    result = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (userIdClaim.Value.Equals(blogPost.AuthorId, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        // TODO: Log here
                        // Basically, the blogPost author is not the one who has been authenticated. return 404 for security reasons.
                        result = Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        string newSlugPath = command.Slug ?? command.Title.ToSlug();
                        Slug existingSlug = blogPost.Slugs.FirstOrDefault(slug => slug.Path.Equals(newSlugPath, StringComparison.InvariantCultureIgnoreCase));
                        IList<Tag> tagsToSave = (command.Tags != null && command.Tags.Any())
                            ? command.Tags.Distinct(StringComparer.InvariantCultureIgnoreCase).Select(tag => new Tag { Name = tag, Slug = tag.ToSlug() }).ToList()
                            : blogPost.Tags.ToList();

                        blogPost.Title = command.Title;
                        blogPost.BriefInfo = command.Summary;
                        blogPost.Content = command.Content;
                        blogPost.Tags = new Collection<Tag>(tagsToSave);
                        blogPost.LastUpdatedOn = DateTimeOffset.Now;

                        if (existingSlug == null)
                        {
                            foreach (Slug slug in blogPost.Slugs) { slug.IsDefault = false; }
                            blogPost.Slugs.Add(new Slug
                            {
                                IsDefault = true,
                                Path = newSlugPath,
                                CreatedOn = DateTimeOffset.Now
                            });
                        }

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new PostModel(blogPost, GetCategoryScheme()));
                        result = response;
                    }
                }
            }

            return result;
        }

        // DELETE api/posts/5
        public async Task<HttpResponseMessage> Delete(int id)
        {
            HttpResponseMessage result;
            ClaimsPrincipal user = User as ClaimsPrincipal;
            Claim userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                BlogPost blogPost = await RavenSession.LoadAsync<BlogPost>(id);
                if (blogPost == null)
                {
                    result = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (userIdClaim.Value.Equals(blogPost.AuthorId, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        // Basically, the blogPost author is not the one who has been authenticated.
                        // return 404 for security reasons.
                        result = Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        RavenSession.Delete(blogPost);
                        result = Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                }
            }

            return result;
        }

        // privates

        private string GetCategoryScheme()
        {
            return Url.Link("DefaultApi", new { controller = "tags" });
        }
    }
}