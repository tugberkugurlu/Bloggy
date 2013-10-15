using Bloggy.Client.Web.Infrastructure.Logging;
using System;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public abstract class RavenController : Controller
    {
        protected readonly IMvcLogger Logger;

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