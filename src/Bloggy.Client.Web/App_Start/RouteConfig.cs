using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bloggy.Client.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ArchiveRoute",
                url: "post/{slug}",
                defaults: new { controller = "BlogPost", action = "Index" }
            );

            routes.MapRoute(
                name: "FeedsByTagRoute",
                url: "tags/{tagSlug}/feeds",
                defaults: new { controller = "Feeds", action = "RssByTagSlug" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
