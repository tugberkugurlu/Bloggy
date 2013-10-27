using System.Net.Http;

namespace Bloggy.Client.Web.Infrastructure.Hypermedia
{
    public interface IResponseEnricher
    {
        bool CanEnrich(HttpResponseMessage response);
        HttpResponseMessage Enrich(HttpResponseMessage response);
    }
}