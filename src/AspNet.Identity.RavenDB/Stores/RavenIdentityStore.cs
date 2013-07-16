using Raven.Client;
using System;

namespace AspNet.Identity.RavenDB.Stores
{
    public abstract class RavenIdentityStore<TUser> where TUser : RavenUser
    {
        protected readonly IAsyncDocumentSession DocumentSession;

        internal protected RavenIdentityStore(IAsyncDocumentSession documentSession)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException("documentSession");
            }

            DocumentSession = documentSession;
        }
    }
}
