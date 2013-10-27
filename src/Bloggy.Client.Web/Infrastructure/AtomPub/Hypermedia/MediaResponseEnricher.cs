using Bloggy.Client.Web.Infrastructure.AtomPub.Models;
using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Hypermedia
{
    public class MediaResponseEnricher : IResponseEnricher
    {
        public bool CanEnrich(HttpResponseMessage response)
        {
            ObjectContent content = response.Content as ObjectContent;

            return content != null
                && (content.ObjectType == typeof(MediaModel));
        }

        public HttpResponseMessage Enrich(HttpResponseMessage response)
        {
            MediaModel media;
            UrlHelper urlHelper = response.RequestMessage.GetUrlHelper();

            if (response.TryGetContentValue<MediaModel>(out media))
            {
                Enrich(media, urlHelper);
            }

            return response;
        }

        private void Enrich(MediaModel media, UrlHelper url)
        {
            string selfUrl = url.Link("DefaultApi", new { controller = "media", id = media.Id });
            media.AddLink(new SelfLink(selfUrl));
            media.AddLink(new EditLink(selfUrl));
        }
    }
}