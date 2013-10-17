using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Domain.Managers;
using Bloggy.Wrappers.Akismet;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
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
                const string DefaultDatabase = "Blog";
                IDocumentStore store = new DocumentStore
                {
                    Url = ravenDbUrl,
                    DefaultDatabase = ravenDefaultDatabase
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(DefaultDatabase);
                return store;

            }).As<IDocumentStore>().SingleInstance();

            builder.RegisterType<NLogLogger>().As<IMvcLogger>().SingleInstance();
            builder.Register(c => Mapper.Engine).As<IMappingEngine>().SingleInstance();
            builder.Register(c => new AkismetClient(akismetApiKey, akismetBlog)).As<AkismetClient>().SingleInstance();
            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();
            builder.RegisterType<BlogManager>().As<IBlogManager>().InstancePerHttpRequest();

            return builder.Build();
        }
    }
}