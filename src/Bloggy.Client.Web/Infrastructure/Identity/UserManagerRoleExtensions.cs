using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class UserManagerRoleExtensions
    {
        public static async Task<bool> IsInRoleAsync<TUser>(this UserManager<TUser> manager, string userId, string role) where TUser : IUser
        {
            IEnumerable<Claim> claims = await manager.GetClaimsAsync(userId);
            return claims.Any(claim => claim.Type == manager.ClaimsIdentityFactory.RoleClaimType && claim.Value == role);
        }
    }
}