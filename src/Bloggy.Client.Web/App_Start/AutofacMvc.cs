using AspNet.Identity.RavenDB.Stores;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Client.Web.Infrastructure.Managers;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Bloggy.Wrappers.Akismet;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using System.Reflection;
using System.Web.Mvc;

namespace Bloggy.Client.Web
{
    public static class AutofacMvc
    {
        public static void Initialize()
        {
            ContainerBuilder builder = new ContainerBuilder();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(RegisterServices(builder)));
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.Register(c =>
            {
                IConfigurationManager configManager = c.Resolve<IConfigurationManager>();
                IDocumentStore store = new DocumentStore
                {
                    Url = configManager.RavenDbUrl,
                    DefaultDatabase = configManager.RavenDbDefaultDatabase
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(configManager.RavenDbDefaultDatabase);
                IndexCreation.CreateIndexes(typeof(Tags_Count).Assembly, store);

                return store;

            }).As<IDocumentStore>().SingleInstance();
            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();

            builder.RegisterType<DefaultConfigurationManager>().As<IConfigurationManager>().SingleInstance();
            builder.RegisterType<NLogLogger>().As<IMvcLogger>().SingleInstance();
            builder.Register(c => Mapper.Engine).As<IMappingEngine>().SingleInstance();
            builder.Register(c =>
            {
                IConfigurationManager configManager = c.Resolve<IConfigurationManager>();
                return new AkismetClient(configManager.AkismetApiKey, configManager.AkismetBlog);
            }).SingleInstance();

            builder.Register(c => new RavenUserStore<BlogUser>(c.Resolve<IAsyncDocumentSession>(), false)).As<IUserStore<BlogUser>>().InstancePerHttpRequest();
            builder.RegisterType<UserManager<BlogUser>>().InstancePerHttpRequest();

            return builder.Build();
        }
    }
}