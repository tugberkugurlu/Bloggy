using Bloggy.Client.Web.Infrastructure.AtomPub.Models;
using Bloggy.Client.Web.Infrastructure.Managers;
using Bloggy.Domain.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bloggy.Client.Web.Controllers.AtomPub
{
    [Authorize(Roles = ApplicationRoles.AdminRole)]
    public class MediaController : RavenApiController
    {
        private const string BlogMediaRavenCollectionName = "BlogMedias";
        private const string ImagesAzureBlobContainerName = "bloggyimages";
        private readonly IPictureManager _pictureManager;

        public MediaController(IAsyncDocumentSession documentSession, IPictureManager pictureManager)
            : base(documentSession)
        {
            _pictureManager = pictureManager;
        }

        private static readonly ReadOnlyDictionary<string, string> MimeTypes = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) { 
                { "image/png", ".png" },
                { "image/jpeg", ".jpg" },
                { "image/gif", ".gif" }
            });

        public async Task<HttpResponseMessage> GetMedia(string id)
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
                string ravenId = string.Concat(BlogMediaRavenCollectionName, "/", id);
                BlogMedia blogMedia = await RavenSession.LoadAsync<BlogMedia>(ravenId);
                if (blogMedia == null)
                {
                    result = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    if (userIdClaim.Value.Equals(blogMedia.AuthorId, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        // TODO: Log here
                        // Basically, the media author is not the one who has been authenticated. return 404 for security reasons.
                        result = Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        MediaModel media = new MediaModel
                        {
                            Id = id,
                            AuthorName = user.Identity.Name,
                            ContentType = blogMedia.ContentType,
                            ImageUrl = new Uri(blogMedia.MediaUrl, UriKind.Absolute),
                            LastUpdated = blogMedia.LastUpdatedOn
                        };

                        result = Request.CreateResponse(HttpStatusCode.OK, media);
                    }
                }
            }

            return result;
        }

        public async Task<HttpResponseMessage> PostMedia()
        {
            // TODO: Try to solve the retrieval problem with a custom Parameter Binding impl.

            HttpResponseMessage result;
            ClaimsPrincipal user = User as ClaimsPrincipal;
            Claim userIdClaim = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                result = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            else
            {
                // Check if there is anything inside the message body
                if (Request.Content != null && Request.Content.Headers.ContentLength > 0)
                {
                    string contentType = Request.Content.Headers.ContentType.MediaType;
                    string extension = GetExtension(contentType);

                    if (string.IsNullOrEmpty(extension) == true)
                    {
                        result = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        string pictureUrl = null;
                        string imageId = Guid.NewGuid().ToString();
                        string fileName = string.Concat(imageId, extension);
                        using (Stream contentStream = await Request.Content.ReadAsStreamAsync())
                        {
                            contentStream.Seek(0, SeekOrigin.Begin);
                            try
                            {
                                pictureUrl = await _pictureManager.UploadAsync(contentStream, ImagesAzureBlobContainerName, fileName, contentType);
                            }
                            catch (Exception ex)
                            {
                                // TODO: Log here.
                            }
                        }

                        if (string.IsNullOrEmpty(pictureUrl) == true)
                        {
                            result = Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                        else
                        {
                            BlogMedia blogMedia = new BlogMedia
                            {
                                Id = string.Concat(BlogMediaRavenCollectionName, "/", imageId),
                                AuthorId = userIdClaim.Value,
                                ContentType = contentType,
                                MediaUrl = pictureUrl,
                                CreatedOn = DateTimeOffset.Now,
                                LastUpdatedOn = DateTimeOffset.Now
                            };

                            MediaModel mediaModel = new MediaModel
                            {
                                Id = imageId,
                                AuthorName = user.Identity.Name,
                                ImageUrl = new Uri(pictureUrl, UriKind.Absolute),
                                ContentType = contentType,
                                LastUpdated = DateTimeOffset.Now
                                // Title = "Awesome Pic Title...",
                                // Summary = "Awesome Pic Summary...",
                            };

                            await RavenSession.StoreAsync(blogMedia);
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, mediaModel);
                            Uri selfLink = new Uri(Url.Link("DefaultApi", new { controller = "media", id = imageId }));
                            response.Headers.Location = selfLink;

                            result = response;
                        }
                    }
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.Conflict);
                }
            }

            return result;
        }

        // privates

        private string GetExtension(string mimeType)
        {
            return MimeTypes.FirstOrDefault(x => x.Key.Equals(mimeType)).Value;
        }
    }
}