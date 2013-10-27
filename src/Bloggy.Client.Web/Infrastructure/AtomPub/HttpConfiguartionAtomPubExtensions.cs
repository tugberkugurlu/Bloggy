using Bloggy.Client.Web.Infrastructure.AtomPub.Dispatchers;
using System.Web.Http;

namespace Bloggy.Client.Web
{
    public static class HttpConfiguartionAtomPubExtensions
    {
        public static void RegisterAtomPubServiceDocument(this HttpConfiguration config, string path)
        {
            config.Routes.MapHttpRoute(
                "__AtomPubServicesRoute",
                path,
                defaults: null,
                constraints: null,
                handler: new AtomPubServiceDocumentDispatcher()
            );
        }
    }
}