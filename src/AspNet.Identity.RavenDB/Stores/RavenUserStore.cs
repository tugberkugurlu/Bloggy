using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenUserStore<TUser> : RavenIdentityStore<TUser>, IUserStore where TUser : RavenUser
    {
        public RavenUserStore(IAsyncDocumentSession documentSession) : base(documentSession)
        {
        }

        public async Task<IUser> Find(string userId)
        {
            return await DocumentSession.LoadAsync<TUser>(userId).ConfigureAwait(false);
        }

        public async Task<IUser> FindByUserName(string userName)
        {
            IList<TUser> users = await DocumentSession.Query<TUser>()
                .Where(usr => usr.UserName == userName)
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }

        public async Task<bool> Create(IUser user)
        {
            TUser tUser = user as TUser;
            if (tUser == null)
            {
                return false;
            }

            await DocumentSession.StoreAsync(tUser).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> Delete(string userId)
        {
            IUser user = await Find(userId).ConfigureAwait(false);
            TUser tUser = user as TUser;
            if (tUser == null)
            {
                return false;
            }

            DocumentSession.Delete<TUser>(tUser);
            return true;
        }
    }
}