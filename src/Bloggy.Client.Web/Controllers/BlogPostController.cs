using AutoMapper;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.RequestModels;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Managers;
using Bloggy.Wrappers.Akismet;
using Bloggy.Wrappers.Akismet.RequestModels;
using Microsoft.Web.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class BlogPostController : RavenController
    {
        private readonly IBlogManager _blogManager;
        private readonly AkismetClient _akismetClient;
        private readonly IMappingEngine _mapper;

        public BlogPostController(IMvcLogger logger, IBlogManager blogManager, AkismetClient akismetClient, IMappingEngine mapper) : base(logger)
        {
            _blogManager = blogManager;
            _akismetClient = akismetClient;
            _mapper = mapper;
        }

        // HTTP GET /archive/{slug}
        public ActionResult Index(string slug)
        {
            return View();
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
            BlogPost blogPost = await _blogManager.GetBlogPostBySlugAsync(slug);
            if(blogPost == null)
            {
                Logger.Warn(string.Format("Blog post could not be found for comment post. Slug: {0}", slug));
                return HttpNotFound();
            }

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
                    await _blogManager.AddCommentAsync(blogPost.Id, blogPostComment);
                    ViewBag.IsCommentSuccess = true;
                    return View();
                }
            }

            return View(requestModel);
        }

        // private helpers

        private BlogPostComment ConstructBlogPostComment(BlogPost blogPost, CommentPostRequestModel requestModel, bool isSpam)
        {
            BlogPostComment blogPostComment = _mapper.Map<CommentPostRequestModel, BlogPostComment>(requestModel);
            blogPostComment.IsApproved = !isSpam;
            blogPostComment.IsSpam = isSpam;
            blogPostComment.IsByAuthor = false;
            blogPostComment.CreationIp = Request.UserHostAddress;
            blogPostComment.LastUpdateIp = Request.UserHostAddress;

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