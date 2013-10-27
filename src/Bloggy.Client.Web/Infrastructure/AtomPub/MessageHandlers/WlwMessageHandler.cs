using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.MessageHandlers
{
    public class WlwMessageHandler : DelegatingHandler
    {
        private const string WlwUserAgent = "Windows Live Writer 1.0";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string headers = request.Headers.ToString();
            string contHeaders = request.Headers.ToString();

            if (request.Headers.UserAgent != null &&
                request.Headers.UserAgent.Any(a => a.Comment != null && a.Comment.Contains(WlwUserAgent)))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/atom+xml"));
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}