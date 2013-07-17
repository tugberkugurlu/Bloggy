using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Utils;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenUserSecretStore<TUser, TUserSecret> : RavenIdentityStore<TUser>, IUserSecretStore
        where TUser : RavenUser where TUserSecret : UserSecret
    {
        public RavenUserSecretStore(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        public async Task<IUserSecret> Find(string userName)
        {
            IEnumerable<UserSecret> userSecrets = await DocumentSession.Query<TUser>()
                .Where(user => user.UserName == userName)
                .Take(1)
                .Select(user => user.Secret)
                .ToListAsync()
                .ConfigureAwait(false);

            return userSecrets.FirstOrDefault();
        }

        public async Task<bool> Create(IUserSecret userSecret)
        {
            bool result;
            TUserSecret tUserSecret = userSecret as TUserSecret;
            TUser user = await GetUser(userSecret.UserName);

			if (tUserSecret == null || user == null || user.Secret != null)
			{
				result = false;
			}
            else
            {
                tUserSecret.Secret = Crypto.HashPassword(tUserSecret.Secret);
                user.Secret = tUserSecret;
                result = true;
            }

            return result;
        }

        public async Task<bool> Update(string userName, string newSecret)
        {
            bool result;
            TUser user = await GetUser(userName);
			
			if (user != null && user.Secret != null)
			{
				user.Secret.Secret = Crypto.HashPassword(newSecret);
				result = true;
			}
			else
			{
				result = false;
			}

			return result;
        }

        public async Task<bool> Delete(string userName)
        {
            bool result;
            TUser user = await GetUser(userName);

            if (user != null && user.Secret != null)
            {
                user.Secret = null;
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> Validate(string userName, string loginSecret)
        {
            bool result;
            TUser user = await GetUser(userName);
            if (user != null && user.Secret != null)
            {
                result = Crypto.VerifyHashedPassword(user.Secret.Secret, loginSecret);
            }
            else
            {
                result = false;
            }

            return result;
        }

        // privates

        private async Task<TUser> GetUser(string userName)
        {
            IEnumerable<TUser> users = await DocumentSession.Query<TUser>()
                .Where(user => user.UserName == userName)
                .Take(1)
                .ToListAsync()
                .ConfigureAwait(false);

            return users.FirstOrDefault();
        }
    }
}