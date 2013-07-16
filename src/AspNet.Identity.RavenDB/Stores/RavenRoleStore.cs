using AspNet.Identity.RavenDB.Indexes;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.RavenDB.Stores
{
    public class RavenRoleStore<TUser> : RavenIdentityStore<TUser>, IRoleStore where TUser : RavenUser
    {
        public RavenRoleStore(IAsyncDocumentSession documentSession) : base(documentSession)
        {
        }

        public async Task<bool> IsUserInRole(string userId, string roleId)
        {
            bool result = await (from user in DocumentSession.Query<TUser>()
                                 where user.Id == userId && user.Roles.Any(role => role.Id == roleId)
                                 select user).AnyAsync();

            return result;
        }

        public async Task<IEnumerable<string>> GetRolesForUser(string userId)
        {
            IEnumerable<string> roles = await DocumentSession.Query<TUser>()
                .Where(user => user.Id == userId)
                .SelectMany(user => user.Roles.Select(role => role.Id))
                .ToListAsync()
                .ConfigureAwait(false);

            return roles;
        }

        public Task<IEnumerable<string>> GetUsersInRoles(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToRole(string roleId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateRole(IRole role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRole(string roleId, bool failIfNonEmpty)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRole(string roleId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RoleExists(string roleId)
        {
            bool result = await DocumentSession.Query<RavenUser_Roles.ReduceResult, RavenUser_Roles>()
                .Where(role => role.Name == roleId).AnyAsync();

            return result;
        }
    }
}
