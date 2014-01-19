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
                name: "BlogPostRoute",
                url: "post/{slug}",
                defaults: new { controller = "BlogPost", action = "Index" }
            );

            routes.MapRoute(
                name: "FeedsByTagRoute",
                url: "tags/{tagSlug}/feeds",
                defaults: new { controller = "Feeds", action = "RssByTagSlug" }
            );

            routes.MapRoute(
                name: "TagsFacetRoute",
                url: "tags/facets",
                defaults: new { controller = "tags", action = "facets" }
            );

            routes.MapRoute(
                name: "TagsRoute",
                url: "tags/{slug}",
                defaults: new { controller = "tags", action = "index" }
            );

            routes.MapRoute(
                name: "ArchiveListRoute",
                url: "archive/{month}/{year}",
                defaults: new { controller = "archive", action = "index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
