using Bloggy.Client.Web.Infrastructure.AtomPub.Formatters;
using Bloggy.Client.Web.Infrastructure.AtomPub.MessageHandlers;
using Bloggy.Client.Web.Infrastructure.Filters;
using Bloggy.Client.Web.Infrastructure.Hypermedia.MessageHandlers;
using Bloggy.Client.Web.Infrastructure.MessageHandlers;
using System.Web.Http;

namespace Bloggy.Client.Web
{
    internal static class AtopPubSampleConfigExtensions
    {
        internal static void RegisterRoutes(this HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
        }

        internal static void RegisterFilters(this HttpConfiguration config)
        {
            config.Filters.Add(new InvalidModelStateFilterAttribute());
        }

        internal static void RegisterMessageHandlers(this HttpConfiguration config)
        {
            config.MessageHandlers.Add(new WlwMessageHandler());
            config.MessageHandlers.Add(new BloggyBasicAuthHandler());
            config.MessageHandlers.Add(new EnrichingHandler());
        }

        internal static void ConfigureFormatters(this HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new AtomPubMediaFormatter());
            config.Formatters.Add(new AtomPubCategoryMediaTypeFormatter());
        }
    }
}