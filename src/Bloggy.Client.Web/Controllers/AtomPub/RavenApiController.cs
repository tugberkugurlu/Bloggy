using Raven.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Bloggy.Client.Web.Controllers.AtomPub
{
    public abstract class RavenApiController : ApiController
    {
        protected readonly IAsyncDocumentSession RavenSession;

        protected RavenApiController(IAsyncDocumentSession documentSession)
        {
            RavenSession = documentSession;
        }

        public async override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.ExecuteAsync(controllerContext, cancellationToken);
            await RavenSession.SaveChangesAsync();

            return response;
        }
    }
}