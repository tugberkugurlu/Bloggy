using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB
{
    public class RavenIdentityStoreContext : IIdentityStoreContext
    {
        private readonly IAsyncDocumentSession _documentSession;

        public RavenIdentityStoreContext(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public IUserStore Users
        {
            get 
            {
                return new RavenUserStore<RavenUser>(_documentSession);
            }
        }

        public IRoleStore Roles
        {
            get { throw new NotImplementedException(); }
        }

        public IUserLoginStore Logins
        {
            get { throw new NotImplementedException(); }
        }

        public IUserSecretStore Secrets
        {
            get { throw new NotImplementedException(); }
        }

        public IUserClaimStore UserClaims
        {
            get { throw new NotImplementedException(); }
        }

        public Task SaveChanges()
        {
            return _documentSession.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
