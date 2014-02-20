using Bloggy.Domain.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Bloggy.Client.Web.Infrastructure.MessageHandlers
{
    // refer to: https://github.com/pmhsfelix/WebApi.AuthenticationFilter.Basic/tree/master/src/WebApi.AuthenticationFilter.Basic
    public class BasicAuthenticationFilter : IAuthenticationFilter
    {
        private readonly Func<string, string, HttpRequestMessage, Task<IPrincipal>> _validate;
        private readonly string _realm;
        public bool AllowMultiple { get { return false; } }

        public BasicAuthenticationFilter(string realm, Func<string, string, HttpRequestMessage, Task<IPrincipal>> validate)
        {
            _validate = validate;
            _realm = "realm=" + realm;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;
            if (HasAuthorizationHeaderWithBasicScheme(req))
            {
                var principal = await TryValidateCredentialsAndCreatePrincipal(req.Headers.Authorization.Parameter, req);
                if (principal != null)
                {
                    context.Principal = principal;
                }
                else
                {
                    // challenges will be added by the ChallengeAsync
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new ActionResultDelegate(context.Result, async (ct, next) =>
            {
                var res = await next.ExecuteAsync(ct);
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    res.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", _realm));
                }
                return res;
            });
            return Task.FromResult<object>(null);
        }

        private bool HasAuthorizationHeaderWithBasicScheme(HttpRequestMessage req)
        {
            return req.Headers.Authorization != null
                   && req.Headers.Authorization.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<IPrincipal> TryValidateCredentialsAndCreatePrincipal(string creds, HttpRequestMessage request)
        {
            string pair;
            try
            {
                pair = Encoding.UTF8.GetString(Convert.FromBase64String(creds));
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            var ix = pair.IndexOf(':');
            if (ix == -1) return null;
            var username = pair.Substring(0, ix);
            var pw = pair.Substring(ix + 1);
            return await _validate(username, pw, request);
        }
    }

    public class BloggyBasicAuthenticationFilter : BasicAuthenticationFilter
    {
        public BloggyBasicAuthenticationFilter()
            : base("MyRealm", async (username, password, request) => 
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

            })
        {
        }
    }

    public class ActionResultDelegate : IHttpActionResult
    {
        private readonly IHttpActionResult _next;
        private readonly Func<CancellationToken, IHttpActionResult, Task<HttpResponseMessage>> _func;

        public ActionResultDelegate(
            IHttpActionResult next,
            Func<CancellationToken, IHttpActionResult, Task<HttpResponseMessage>> func)
        {
            _next = next;
            _func = func;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return _func(cancellationToken, _next);
        }
    }
}