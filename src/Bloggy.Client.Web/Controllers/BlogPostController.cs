using AutoMapper;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.RequestModels;
using Bloggy.Client.Web.ViewModels;
using Bloggy.Domain.Entities;
using Bloggy.Wrappers.Akismet;
using Bloggy.Wrappers.Akismet.RequestModels;
using Microsoft.Web.Helpers;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class BlogPostController : RavenController
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly AkismetClient _akismetClient;
        private readonly IMappingEngine _mapper;

        public BlogPostController(IMvcLogger logger, IAsyncDocumentSession documentSession, AkismetClient akismetClient, IMappingEngine mapper) : base(logger)
        {
            _documentSession = documentSession;
            _akismetClient = akismetClient;
            _mapper = mapper;
        }

        // HTTP GET /archive/{slug}
        public async Task<ActionResult> Index(string slug)
        {
            // TODO: 1-) Try to retrieve the blog pots by the slug ( > Slugs.Contains(slug)).
            //       2-) If not found, return Not Found.
            //       3-) If found, look for the default slug.
            //           3.0-) If the default slug is not found, log it and return NotFound.
            //           3.1-) If it's the default slug, retrieve the comments and return the view model.
            //           3.2-) If it's not the default slug, redirect to its default slug.
            //       4-) If found by the alternative slug, redirect it to default slug.

            // 1-) Try to retrieve the blog pots by the slug ( > Slugs.Contains(slug)).
            BlogPost blogPost = await RetrieveBlogPostAsync(slug);
            if (blogPost == null)
            {
                 // 2-) If not found, return Not Found.
                Logger.Warn(string.Format("Blog post could not be found. Slug: {0}", slug));
                return HttpNotFound();
            }

            // 3-) If found, look for the default slug.
            Slug defaultSlug = blogPost.Slugs
                .OrderByDescending(slugEntity => slugEntity.CreatedOn)
                .FirstOrDefault(slugEntity => slugEntity.IsDefault == true);

            if (defaultSlug == null)
            {
                // 3.0-) If the default slug is not found, log it and return NotFound.
                Logger.Warn(string.Format("The default slug could not be found for the blog post. BlogPost Id: {0}, Slug Parameter: {1}", blogPost.Id.ToIntId(), slug));
                return HttpNotFound();
            }

            if (defaultSlug.Path.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                // 3.1-) If it's the default slug, retrieve the comments and return the view model.
                BlogPostPageViewModel viewModel = await ConstructBlogPostViewModelWithCommentsAsync(blogPost, defaultSlug.Path);
                return View(viewModel);
            }
            else
            {
                // 3.2-) If it's not the default slug, redirect to its default slug.
                Logger.Warn(string.Format("Blog post is found for the slug '{0}' but it's not the default slug. Redirection to slug '{1}'. BlogPost Id: {2}", slug, defaultSlug.Path, blogPost.Id.ToIntId()));
                return RedirectToActionPermanent("Index", new { slug = defaultSlug.Path });
            }
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        // HTTP POST /archive/{slug}
        public async Task<ActionResult> CreateBlogComment(string slug, CommentPostRequestModel requestModel)
        {
            // TODO: 1-) Check whether the post/dynamic-page exists and comments enabled for that.
            //       2-) Check whether model state is valid.
            //       3-) Check  whether captcha is valid.
            //       4-) Check whether the spam check is enabled or not. If enabled, check against spam.
            //           4.1-) If spam, log the info as warning and mark the comment as spam.
            //           4.2-) If spam check throws an exception, log the error message and continue our way as the comment is not spam.
            //       5-) If not spam, write the comment as OK into the right place.

            // 1-) Check whether the post/dynamic-page exists and comments enabled for that.
            BlogPost blogPost = await RetrieveBlogPostAsync(slug);
            if(blogPost == null)
            {
                Logger.Warn(string.Format("Blog post could not be found for comment post. Slug: {0}", slug));
                return HttpNotFound();
            }

            BlogPostPageViewModel viewModel = await ConstructBlogPostViewModelWithCommentsAsync(blogPost, slug);

            // 2-) Blog post exists. Check whether model state is valid.
            if (ModelState.IsValid)
            {
                // 3-) Model State is valid. Check  whether captcha is valid.
                bool isCaptchaActive = bool.Parse(ConfigurationManager.AppSettings[Constants.RecaptchaActiveAppSettingsKey]);
                if (isCaptchaActive && !ReCaptcha.Validate(ConfigurationManager.AppSettings[Constants.RecaptchaPrivateKeyAppSettingsKey]))
                {
                    ModelState.AddModelError(string.Empty, ValidationResources.Captcha);
                }
                else 
                {
                    // 4-) Check whether the spam check is enabled or not. If enabled, check against spam.
                    bool isSpam = false;
                    bool isSpamCheckEnabled = bool.Parse(ConfigurationManager.AppSettings[Constants.AkismetActiveAppSettingsKey]);
                    if (isSpamCheckEnabled)
                    {
                        // Spam check enabled. Check against spam.
                        isSpam = await CheckCommentIfSpamAsync(slug, requestModel);
                    }

                    BlogPostComment blogPostComment = ConstructBlogPostComment(blogPost, requestModel, isSpam);
                    await _documentSession.StoreAsync(blogPostComment);
                    await _documentSession.SaveChangesAsync();
                    ViewBag.IsCommentSuccess = true;

                    return View(viewModel);
                }
            }

            viewModel.CommentPostRequestModel = requestModel;
            return View(viewModel);
        }

        // private helpers

        private async Task<BlogPost> RetrieveBlogPostAsync(string slug)
        {
            IEnumerable<BlogPost> blogPosts = await _documentSession.Query<BlogPost>()
                .Where(post => post.Slugs.Any(slugEntity => slugEntity.Path == slug))
                .Take(1)
                .ToListAsync();

            BlogPost blogPost = blogPosts.FirstOrDefault();
            return blogPost;
        }

        private async Task<BlogPostPageViewModel> ConstructBlogPostViewModelWithCommentsAsync(BlogPost blogPost, string defaultSlug)
        {
            IEnumerable<BlogPostComment> comments = await _documentSession.Query<BlogPostComment>()
                .OrderBy(comment => comment.CreatedOn)
                .Where(comment => comment.BlogPostId == blogPost.Id && comment.IsSpam == false && comment.IsApproved == true)
                .ToListAsync();

            BlogPostPageViewModel viewModel = new BlogPostPageViewModel
            {
                BlogPost = _mapper.Map<BlogPost, BlogPostModelLight>(blogPost),
                Comments = _mapper.Map<IEnumerable<BlogPostComment>, IEnumerable<BlogPostCommentModel>>(comments)
            };

            viewModel.BlogPost.Slug = defaultSlug;

            return viewModel;
        }

        private BlogPostComment ConstructBlogPostComment(BlogPost blogPost, CommentPostRequestModel requestModel, bool isSpam)
        {
            BlogPostComment blogPostComment = _mapper.Map<CommentPostRequestModel, BlogPostComment>(requestModel);
            blogPostComment.BlogPostId = blogPost.Id;
            blogPostComment.IsApproved = !isSpam;
            blogPostComment.IsSpam = isSpam;
            blogPostComment.IsByAuthor = false;
            blogPostComment.CreationIp = Request.UserHostAddress;
            blogPostComment.LastUpdateIp = Request.UserHostAddress;
            blogPostComment.CreatedOn = DateTimeOffset.Now;
            blogPostComment.LastUpdatedOn = DateTimeOffset.Now;

            if (Identity != null && Identity.IsAuthenticated)
            {
                string userId = RetrieveUserId(Identity);
                if (userId == blogPost.AuthorId)
                {
                    blogPostComment.IsByAuthor = true;
                }
            }

            return blogPostComment;
        }

        private async Task<bool> CheckCommentIfSpamAsync(string slug, CommentPostRequestModel requestModel)
        {
            bool isSpam = true;
            try
            {
                AkismetCommentRequestModel akismetComment = ConstructAkismetComment(requestModel);
                AkismetResponse<bool> akismetResponse = await _akismetClient.CheckCommentAsync(akismetComment);
                if (akismetResponse.IsSuccessStatusCode)
                {
                    isSpam = !akismetResponse.Entity;
                }
                else
                {
                    Logger.Error(string.Format("Akismet spam check didn't return success status code for bog post '{0}'. Status Code: {1}, Error Message: {2}", slug, (int)akismetResponse.Status, akismetResponse.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error occured during the spam check for bog post '{0}'", slug), ex);
            }

            return isSpam;
        }

        private AkismetCommentRequestModel ConstructAkismetComment(CommentPostRequestModel requestModel)
        {
            AkismetCommentRequestModel akismetComment = _mapper.Map<CommentPostRequestModel, AkismetCommentRequestModel>(requestModel);
            akismetComment.CommentType = "Comment";
            akismetComment.Permalink = Request.Url.ToString();
            akismetComment.Referrer = Request.UrlReferrer.ToString();
            akismetComment.UserAgent = Request.UserAgent;
            akismetComment.UserIp = Request.UserHostName;

            return akismetComment;
        }

        private string RetrieveUserId(ClaimsIdentity identity)
        {
            string result = null;
            Claim claim = identity.Claims.FirstOrDefault(claimEntity => claimEntity.Type == ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                result = claim.Value;
            }

            return result;
        }
    }
}