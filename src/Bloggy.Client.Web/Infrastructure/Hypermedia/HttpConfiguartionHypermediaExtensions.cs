using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System.Collections.ObjectModel;
using System.Web.Http;

namespace Bloggy.Client.Web
{
    public static class HttpConfiguartionHypermediaExtensions
    {
        public static void AddResponseEnrichers(this HttpConfiguration config, params IResponseEnricher[] enrichers)
        {
            foreach (var enricher in enrichers)
            {
                config.GetResponseEnrichers().Add(enricher);
            }
        }

        public static Collection<IResponseEnricher> GetResponseEnrichers(this HttpConfiguration config)
        {
            return (Collection<IResponseEnricher>)config.Properties.GetOrAdd(
                typeof(Collection<IResponseEnricher>), k => new Collection<IResponseEnricher>());
        }
    }
}