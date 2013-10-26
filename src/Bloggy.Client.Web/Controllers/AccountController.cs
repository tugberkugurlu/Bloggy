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
    [Authorize]
    public class AccountController : RavenController
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly UserManager<BlogUser> _userManager;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(IMvcLogger logger, IAsyncDocumentSession documentSession, UserManager<BlogUser> userManager) : base(logger)
        {
            _documentSession = documentSession;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> CreateManagementUser()
        {
            // TODO: 1-) Check if any user exists inside the DB.
            //       2-) If exists, return 404.
            //       3-) If not exists, return the view.

            bool anyUserExists = await _documentSession.Query<BlogUser>().AnyAsync();
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
            bool anyUserExists = await _documentSession.Query<BlogUser>().AnyAsync();
            if (anyUserExists)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                BlogUser blogUser = new BlogUser
                {
                    UserName = requestModel.Username,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                blogUser.Claims.Add(new RavenUserClaim { ClaimType = ClaimTypes.Email, ClaimValue = requestModel.Email });
                blogUser.Claims.Add(new RavenUserClaim { ClaimType = _userManager.ClaimsIdentityFactory.RoleClaimType, ClaimValue = ApplicationRoles.AdminRole });
                IdentityResult result = await _userManager.CreateAsync(blogUser, requestModel.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(blogUser, isPersistent: false);
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form.
            return View(requestModel);
        }

        public ActionResult ChangePassword()
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
                return RedirectToAction("Index", "Home");
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