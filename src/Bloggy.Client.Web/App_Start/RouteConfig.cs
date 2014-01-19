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
                defaults: new { controller = "archive", action = "index" },
                constraints: new { month = new System.Web.Mvc.Routing.Constraints.IntRouteConstraint(), year = new System.Web.Mvc.Routing.Constraints.IntRouteConstraint() }
            );

            routes.MapRoute(
                name: "BlogPostRoute",
                url: "archive/{slug}",
                defaults: new { controller = "BlogPost", action = "Index" }
            );

            routes.MapRoute(
                name: "AboutRoute",
                url: "about",
                defaults: new { controller = "default", action = "about" }
            );

            routes.MapRoute(
                name: "ContactRoute",
                url: "contact",
                defaults: new { controller = "default", action = "contact" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
