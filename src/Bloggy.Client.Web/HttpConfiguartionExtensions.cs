using System.Collections.ObjectModel;
using System.Web.Http;
using Bloggy.Client.Web.Infrastructure.AtomPub.Dispatchers;
using Bloggy.Client.Web.Infrastructure.Hypermedia;

namespace Bloggy.Client.Web
{
    public static class HttpConfiguartionExtensions
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

        public static void AddResponseEnrichers(this HttpConfiguration config, params IResponseEnricher[] enrichers)
        {
            foreach (var enricher in enrichers)
            {
                GetResponseEnrichers(config).Add(enricher);
            }
        }

        public static Collection<IResponseEnricher> GetResponseEnrichers(this HttpConfiguration config)
        {
            return (Collection<IResponseEnricher>)config.Properties.GetOrAdd(
                typeof(Collection<IResponseEnricher>),
                k => new Collection<IResponseEnricher>()
                );
        }
    }
}