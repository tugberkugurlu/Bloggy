using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Bloggy.Client.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.unobtrusive*")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/content/css")
                .Include("~/Content/less/bootstrap.css")
                .Include("~/Content/font-awesome.css")
                .Include("~/Content/bloggy-main.css")
                .Include("~/Content/less/responsive.css"));
        }
    }
}