﻿using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Bloggy.Client.Web.Infrastructure.Logging;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
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
                const string DefaultDatabase = "Blog";
                IDocumentStore store = new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = DefaultDatabase
                }.Initialize();

                store.DatabaseCommands.EnsureDatabaseExists(DefaultDatabase);
                return store;

            }).As<IDocumentStore>().SingleInstance();

            builder.Register(c => Mapper.Engine).As<IMappingEngine>().SingleInstance();
            builder.RegisterType<NLogLogger>().As<IMvcLogger>().SingleInstance();
            builder.Register(c => c.Resolve<IDocumentStore>().OpenAsyncSession()).As<IAsyncDocumentSession>().InstancePerHttpRequest();

            return builder.Build();
        }
    }
}