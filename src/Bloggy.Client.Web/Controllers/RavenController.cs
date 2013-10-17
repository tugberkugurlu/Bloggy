using Bloggy.Client.Web.Infrastructure.Logging;
using System;
using System.Security.Claims;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public abstract class RavenController : Controller
    {
        protected readonly IMvcLogger Logger;

        protected virtual ClaimsIdentity Identity 
        {
            get
            {
                return User.Identity as ClaimsIdentity;
            }
        }

        public RavenController(IMvcLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            Logger = logger;
        }
    }
}