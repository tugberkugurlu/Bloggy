using AspNet.Identity.RavenDB.Entities;
using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.RequestModels;
using Bloggy.Domain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Raven.Client;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class AccountController : RavenController
    {
        private readonly UserManager<BlogUser> _userManager;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(IMvcLogger logger, IAsyncDocumentSession documentSession, UserManager<BlogUser> userManager) : base(logger, documentSession)
        {
            _userManager = userManager;
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginPost(LoginRequestModel requestModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                BlogUser user = await _userManager.FindAsync(requestModel.Username, requestModel.Password);
                if (user != null)
                {
                    await SignInAsync(user, requestModel.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(requestModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Default");
        }

        [AllowAnonymous]
        public async Task<ActionResult> CreateManagementUser()
        {
            // TODO: 1-) Check if any user exists inside the DB.
            //       2-) If exists, return 404.
            //       3-) If not exists, return the view.

            bool anyUserExists = await DocumentSession.Query<BlogUser>().AnyAsync();
            if (anyUserExists)
            {
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        [ActionName("CreateManagementUser")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateManagementUserPost(ManagementUserRequestModel requestModel)
        {
            // NOTE: Not a 100% safe way but will work out fine for the purpose of this action.
            bool anyUserExists = await DocumentSession.Query<BlogUser>().AnyAsync();
            if (anyUserExists)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                // refer to: https://groups.google.com/forum/#!searchin/ravendb/tugberk/ravendb/_7r5TcfJwx4/h5j7p4XQgeQJ
                DocumentSession.Advanced.UseOptimisticConcurrency = true;
                BlogUser blogUser = new BlogUser
                {
                    Id = string.Concat("BlogUsers/", requestModel.Username),
                    UserName = requestModel.Username,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                blogUser.Claims.Add(new RavenUserClaim { ClaimType = ClaimTypes.Email, ClaimValue = requestModel.Email });
                blogUser.Claims.Add(new RavenUserClaim { ClaimType = _userManager.ClaimsIdentityFactory.RoleClaimType, ClaimValue = ApplicationRoles.AdminRole });
                IdentityResult result = await _userManager.CreateAsync(blogUser, requestModel.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(blogUser, isPersistent: false);
                    return RedirectToAction("Index", "Default");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form.
            return View(requestModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            return View();
        }

        // private helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Default");
            }
        }

        private async Task SignInAsync(BlogUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            ClaimsIdentity identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }
	}
}