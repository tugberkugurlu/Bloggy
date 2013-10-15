using Bloggy.Client.Web.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    [Authorize]
    public class AccountController : RavenController
    {
        public AccountController(IMvcLogger logger) : base(logger)
        {
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
	}
}