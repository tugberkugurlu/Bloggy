﻿using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Forms;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            app.UseFormsAuthentication(new FormsAuthenticationOptions {

                AuthenticationType = FormsAuthenticationDefaults.ApplicationAuthenticationType,
                AuthenticationMode = AuthenticationMode.Active,
                CookieName = string.Concat(FormsAuthenticationDefaults.CookiePrefix, FormsAuthenticationDefaults.ApplicationAuthenticationType),
                LoginPath = FormsAuthenticationDefaults.LoginPath,
                LogoutPath = FormsAuthenticationDefaults.LogoutPath
            });

            // ASP.NET Specifics
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}