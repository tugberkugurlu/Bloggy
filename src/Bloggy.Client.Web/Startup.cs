using AspNet.Identity.RavenDB.Stores;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using Bloggy.Client.Web.Infrastructure.AtomPub.Hypermedia;
using Bloggy.Client.Web.Infrastructure.Managers;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Owin;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Bloggy.Client.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                AuthenticationMode = AuthenticationMode.Passive
            });

            // ASP.NET WEb API Specifics
            HttpConfiguration config = GlobalConfiguration.Configuration;
            IContainer container = RegisterServices(new ContainerBuilder());
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.RegisterAtomPubServiceDocument("api/services");
            config.AddResponseEnrichers(new PostResponseEnricher());
            config.AddResponseEnrichers(new MediaResponseEnricher());
            config.RegisterRoutes();
            config.RegisterFilters();
            config.RegisterMessageHandlers();
            config.ConfigureFormatters();

            // ASP.NET Specifics
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacMvc.Initialize();
            AutoMapperConfig.Configure();
        }

        private IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
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

                // Create any Facets.
                FacetTags.CreateFacets(store);

                return store;

            }).As<IDocumentStore>().SingleInstance();
            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerApiRequest();

            builder.RegisterType<DefaultConfigurationManager>().As<IConfigurationManager>().SingleInstance();
            builder.Register(c => Mapper.Engine).As<IMappingEngine>().SingleInstance();
            builder.Register(c => new RavenUserStore<BlogUser>(c.Resolve<IAsyncDocumentSession>(), false)).As<IUserStore<BlogUser>>().InstancePerApiRequest();
            builder.RegisterType<UserManager<BlogUser>>().InstancePerApiRequest();

            builder.Register(c => CloudStorageAccount.Parse(c.Resolve<IConfigurationManager>().AzureBlobStorageConnectionString)).As<CloudStorageAccount>().InstancePerApiRequest();
            builder.Register(c => c.Resolve<CloudStorageAccount>().CreateCloudBlobClient()).As<CloudBlobClient>().InstancePerApiRequest();
            builder.RegisterType<AzureBlobStoragePictureManager>().As<IPictureManager>().InstancePerApiRequest();

            return builder.Build();
        }
    }
}