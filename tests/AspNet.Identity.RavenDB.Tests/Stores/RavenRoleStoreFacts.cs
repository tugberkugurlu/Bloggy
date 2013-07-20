using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenRoleStoreFacts : TestBase
    {
        [Fact]
        public async Task GetRolesForUser_Should_Retrieve_Correct_Roles()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/2", UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Admin" } } });
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/3", UserName = "Tugberk3", Roles = new List<Role> { new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                IEnumerable<string> roles = await roleStore.GetRolesForUser("RavenUsers/1");

                Assert.Equal(2, roles.Count());
                Assert.True(roles.Any(role => role.Equals("Admin", StringComparison.InvariantCultureIgnoreCase)));
                Assert.True(roles.Any(role => role.Equals("Guest", StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        [Fact]
        public async Task GetRolesForUser_Should_Return_Empty_Enumerable_If_Roles_Property_Is_Null()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk" });
                await ses.SaveChangesAsync();

                IEnumerable<string> roles = await roleStore.GetRolesForUser("RavenUsers/1");

                Assert.Equal(0, roles.Count());
            }
        }

        [Fact]
        public async Task GetRolesForUser_Should_Return_Empty_Enumerable_If_User_Does_Not_Exist()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                IEnumerable<string> roles = await roleStore.GetRolesForUser("RavenUsers/2");

                Assert.Equal(0, roles.Count());
            }
        }

        [Fact]
        public async Task IsUserInRole_Should_Return_True_If_User_Is_Really_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool isAdmin = await roleStore.IsUserInRole("RavenUsers/1", "Admin");

                Assert.True(isAdmin);
            }
        }

        [Fact]
        public async Task IsUserInRole_Should_Return_False_If_User_Is_Not_In_Role()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool isSalesGuy = await roleStore.IsUserInRole("RavenUsers/1", "SalesGuy");

                Assert.False(isSalesGuy);
            }
        }

        [Fact]
        public async Task IsUserInRole_Should_Return_False_If_User_Does_Not_Exist()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IRoleStore roleStore = new RavenRoleStore<RavenUser, Role>(ses);
                await ses.StoreAsync(new RavenUser { Id = "RavenUsers/1", UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
                await ses.SaveChangesAsync();

                bool isAdmin = await roleStore.IsUserInRole("RavenUsers/2", "Admin");

                Assert.False(isAdmin);
            }
        }

        // privates
        private static async Task AddUsers(IAsyncDocumentSession ses)
        {
            await ses.StoreAsync(new RavenUser { UserName = "Tugberk", Roles = new List<Role> { new Role { Id = "Admin" }, new Role { Id = "Guest" } } });
            await ses.StoreAsync(new RavenUser { UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Admin" } } });
            await ses.StoreAsync(new RavenUser { UserName = "Tugberk2", Roles = new List<Role> { new Role { Id = "Guest" } } });
            await ses.SaveChangesAsync();
        }
    }
}
