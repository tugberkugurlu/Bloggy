using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    public class RavenUserStoreFacts
    {
        [Fact]
        public async Task Should_Create_User()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                IUserStore userStore = new RavenUserStore<RavenUser>(ses);
                bool result = await userStore.Create(new RavenUser { UserName = userName });
                await ses.SaveChangesAsync();

                IUser user = (await ses.Query<RavenUser>()
                    .Where(usr => usr.UserName == userName)
                    .Take(1)
                    .ToListAsync()
                    .ConfigureAwait(false)).FirstOrDefault();

                Assert.True(result);
                Assert.NotNull(user);
            }
        }

        // privates
        private IDocumentStore CreateEmbeddableStore()
        {
            return new EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                }
            }.Initialize();
        }
    }
}
