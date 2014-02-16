using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WebApiDoodle.Web.MessageHandlers;

namespace Bloggy.Client.Web.Infrastructure.MessageHandlers
{
    public class BloggyBasicAuthHandler : BasicAuthenticationHandler
    {
        protected override Task<IPrincipal> AuthenticateUserAsync(HttpRequestMessage request,
            string username, string password, CancellationToken cancellationToken)
        {
            if (username.Equals(password, StringComparison.InvariantCultureIgnoreCase))
            {
                IPrincipal principal = new GenericPrincipal(new GenericIdentity(username), null);
                return Task.FromResult(principal);
            }

            return Task.FromResult<IPrincipal>(null);
        }
    }
}