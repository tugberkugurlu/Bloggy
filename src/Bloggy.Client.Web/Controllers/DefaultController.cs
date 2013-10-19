using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class DefaultController : RavenController
    {
        public DefaultController(IMvcLogger logger) : base(logger)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}