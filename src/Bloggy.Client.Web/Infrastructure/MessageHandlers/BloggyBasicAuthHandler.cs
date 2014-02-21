using Bloggy.Domain.Entities;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace Bloggy.Client.Web.Infrastructure.MessageHandlers
{
    public class BloggyBasicAuthHandler : BasicAuthenticationHandler
    {
        protected override async Task<IPrincipal> AuthenticateUserAsync(HttpRequestMessage request, string username, string password, CancellationToken cancellationToken)
        {
            IPrincipal result = null;
            IDependencyScope dependencyScope = request.GetDependencyScope();
            UserManager<BlogUser> userManager = dependencyScope.GetService(typeof(UserManager<BlogUser>)) as UserManager<BlogUser>;
            BlogUser user = await userManager.FindAsync(username, password);

            if (user != null)
            {
                ClaimsIdentity identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ExternalBearer);
                result = new ClaimsPrincipal(identity);
            }

            return result;
        }
    }
}