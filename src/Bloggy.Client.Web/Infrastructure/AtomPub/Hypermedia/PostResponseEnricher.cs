using Bloggy.Client.Web.Infrastructure.AtomPub.Models;
using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Hypermedia
{
    public class PostResponseEnricher : IResponseEnricher
    {
        public bool CanEnrich(HttpResponseMessage response)
        {
            ObjectContent content = response.Content as ObjectContent;

            return content != null
                && (content.ObjectType == typeof(PostModel) 
                || content.ObjectType == typeof(PostFeed));
        }

        public HttpResponseMessage Enrich(HttpResponseMessage response)
        {
            PostModel post;
            UrlHelper urlHelper = response.RequestMessage.GetUrlHelper();

            if (response.TryGetContentValue<PostModel>(out post))
            {
                Enrich(post, urlHelper);
                return response;
            }

            PostFeed feed;
            if (response.TryGetContentValue<PostFeed>(out feed))
            {
                foreach (var p in feed.Posts)
                {
                    Enrich(p, urlHelper);
                }

                string selfUrl = urlHelper.Link("DefaultApi", new { controller = "posts" });
                feed.AddLink(new SelfLink(selfUrl));
            }

            return response;
        }

        private void Enrich(PostModel post, UrlHelper url)
        {
            string selfUrl = url.Link("DefaultApi", new { controller = "posts", id = post.Id });
            post.AddLink(new SelfLink(selfUrl));
            post.AddLink(new EditLink(selfUrl));
        }
    }
}