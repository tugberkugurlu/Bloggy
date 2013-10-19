using AspNet.Identity.RavenDB.Stores;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Bloggy.Domain.Managers;
using Bloggy.Wrappers.Akismet;
using Microsoft.AspNet.Identity;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using System.Configuration;
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
            string akismetApiKey = ConfigurationManager.AppSettings[Constants.AkismetApiKeyAppSettingsKey];
            string akismetBlog = ConfigurationManager.AppSettings[Constants.AkismetBlogAppSettingsKey];
            string ravenDbUrl = ConfigurationManager.AppSettings[Constants.RavenDbUrlAppSettingsKey];
            string ravenDefaultDatabase = ConfigurationManager.AppSettings[Constants.RavenDbDefaultDatabaseAppSettingsKey];

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.Register(c =>
            {
                IDocumentStore store = new DocumentStore
                {
                    Url = ravenDbUrl,
                    DefaultDatabase = ravenDefaultDatabase
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(ravenDefaultDatabase);
                IndexCreation.CreateIndexes(typeof(Tags_Count).Assembly, store);

                return store;

            }).As<IDocumentStore>().SingleInstance();
            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();

            builder.RegisterType<NLogLogger>().As<IMvcLogger>().SingleInstance();
            builder.Register(c => Mapper.Engine).As<IMappingEngine>().SingleInstance();

            builder.Register(c => new RavenUserStore<User>(c.Resolve<IAsyncDocumentSession>(), false)).As<IUserStore<User>>().InstancePerHttpRequest();
            builder.RegisterType<UserManager<User>>().InstancePerHttpRequest();

            builder.Register(c => new AkismetClient(akismetApiKey, akismetBlog)).SingleInstance();
            builder.RegisterType<BlogManager>().As<IBlogManager>().InstancePerHttpRequest();
            builder.RegisterType<DynamicPageManager>().As<IDynamicPageManager>().InstancePerHttpRequest();

            return builder.Build();
        }
    }
}