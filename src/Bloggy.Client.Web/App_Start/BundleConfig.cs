using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using dotless.Core.Abstractions;
using dotless.Core.Importers;
using dotless.Core.Input;
using dotless.Core.Loggers;
using dotless.Core.Parser;
using dotless.Core;
using System.Text;
using System.Collections.Generic;

namespace Bloggy.Client.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // NOTE: To work with bootstrap less files in this way, read the following articles:
            //       http://benjii.me/2012/10/using-less-css-with-mvc4-web-optimization/
            //       http://stackoverflow.com/questions/9593254/mvc4-less-bundle-import-directory
            //       http://www.tomdupont.net/2013/10/bootstrap-3-less-bundling-and-aspnet-mvc.html
            //       http://stackoverflow.com/questions/13581620/bootstrap-theme-variables-less-bootswatch
            //       http://stackoverflow.com/questions/15252829/how-to-use-asp-net-mvc-4-to-bundle-less-files-in-release-mode
            //       http://ben.onfabrik.com/posts/adding-less-support-to-the-aspnet-optimization-framework

            //ThemeTransformer themeTransformer = new ThemeTransformer();
            //CssTransformer cssTransformer = new CssTransformer();
            //NullOrderer nullOrderer = new NullOrderer();

            //Bundle css = new Bundle("~/content/css").Include("~/Content/bootstrap/bootstrap.less");
            //// css.Transforms.Add(themeTransformer);
            //css.Transforms.Add(cssTransformer);
            //css.Orderer = nullOrderer;
            //bundles.Add(css);

            bundles.Add(new Bundle(
                "~/Content/css",
                new LessBundleTransform(),
                new CssMinify()).Include("~/Content/bootstrap/bootstrap.less"));

            bundles.Add(new ScriptBundle("~/bundles/scripts")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.unobtrusive*")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/bootstrap.js"));
        }
    }

    public class VirtualFileReader : IFileReader
    {
        public IPathResolver PathResolver { get; set; }

        public VirtualFileReader(IPathResolver pathResolver)
        {
            PathResolver = pathResolver;
        }

        /// <summary>
        /// Returns the binary contents of the specified file.
        /// </summary>
        /// <param name="fileName">The relative, absolute or virtual file path.</param>
        /// <returns>The contents of the specified file as a binary array.</returns>
        public byte[] GetBinaryFileContents(string fileName)
        {
            fileName = PathResolver.GetFullPath(fileName);

            var virtualPathProvider = HostingEnvironment.VirtualPathProvider;
            var virtualFile = virtualPathProvider.GetFile(fileName);
            using (var stream = virtualFile.Open())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Returns the string contents of the specified file.
        /// </summary>
        /// <param name="fileName">The relative, absolute or virtual file path.</param>
        /// <returns>The contents of the specified file as string.</returns>
        public string GetFileContents(string fileName)
        {
            fileName = PathResolver.GetFullPath(fileName);

            var virtualPathProvider = HostingEnvironment.VirtualPathProvider;
            var virtualFile = virtualPathProvider.GetFile(fileName);
            using (var streamReader = new StreamReader(virtualFile.Open()))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns a value that indicates if the specified file exists.
        /// </summary>
        /// <param name="fileName">The relative, absolute or virtual file path.</param>
        /// <returns>True if the file exists, otherwise false.</returns>
        public bool DoesFileExist(string fileName)
        {
            fileName = PathResolver.GetFullPath(fileName);

            var virtualPathProvider = HostingEnvironment.VirtualPathProvider;
            return virtualPathProvider.FileExists(fileName);
        }

        public bool UseCacheDependencies { get { return false; } }
    }

    public class VirtualPathResolver : IPathResolver
    {
        private string currentFileDirectory;
        private string currentFilePath;

        public VirtualPathResolver(string currentFilePath)
        {
            CurrentFilePath = currentFilePath;
        }

        /// <summary>
        /// Gets or sets the path to the currently processed file.
        /// </summary>
        public string CurrentFilePath
        {
            get { return currentFilePath; }
            set
            {
                currentFilePath = value;
                currentFileDirectory = VirtualPathUtility.GetDirectory(value);
            }
        }

        /// <summary>
        /// Returns the virtual path for the specified file <param name="path"/>.
        /// </summary>
        /// <param name="path">The imported file path.</param>
        public string GetFullPath(string path)
        {
            if (path[0] == '~') // a virtual path e.g. ~/assets/style.less
            {
                return path;
            }

            if (VirtualPathUtility.IsAbsolute(path)) // an absolute path e.g. /assets/style.less
            {
                return VirtualPathUtility.ToAppRelative(path,
                    HostingEnvironment.IsHosted ? HostingEnvironment.ApplicationVirtualPath : "/");
            }

            // otherwise, assume relative e.g. style.less or ../../variables.less
            return VirtualPathUtility.Combine(currentFileDirectory, path);
        }
    }

    public class LessBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse bundle)
        {
            context.HttpContext.Response.Cache.SetLastModifiedFromFileDependencies();

            var lessParser = new Parser();
            ILessEngine lessEngine = CreateLessEngine(lessParser);

            var content = new StringBuilder();

            var bundleFiles = new List<BundleFile>();

            foreach (var bundleFile in bundle.Files)
            {
                bundleFiles.Add(bundleFile);

                SetCurrentFilePath(lessParser, bundleFile.VirtualFile.VirtualPath);

                using (var reader = new StreamReader(VirtualPathProvider.OpenFile(bundleFile.VirtualFile.VirtualPath)))
                {
                    content.Append(lessEngine.TransformToCss(reader.ReadToEnd(), bundleFile.VirtualFile.VirtualPath));
                    content.AppendLine();

                    bundleFiles.AddRange(GetFileDependencies(lessParser));
                }
            }

            if (BundleTable.EnableOptimizations)
            {
                // include imports in bundle files to register cache dependencies
                bundle.Files = bundleFiles.Distinct().ToList();
            }

            bundle.ContentType = "text/css";
            bundle.Content = content.ToString();
        }

        /// <summary>
        /// Creates an instance of LESS engine.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        private ILessEngine CreateLessEngine(Parser lessParser)
        {
            var logger = new AspNetTraceLogger(LogLevel.Debug, new Http());
            return new LessEngine(lessParser, logger, true, false);
        }

        /// <summary>
        /// Gets the file dependencies (@imports) of the LESS file being parsed.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        /// <returns>An array of file references to the dependent file references.</returns>
        private IEnumerable<BundleFile> GetFileDependencies(Parser lessParser)
        {
            var pathResolver = GetPathResolver(lessParser);

            foreach (var importPath in lessParser.Importer.Imports)
            {
                string fullPath = pathResolver.GetFullPath(importPath);
                yield return new BundleFile(fullPath, HostingEnvironment.VirtualPathProvider.GetFile(fullPath));
            }

            lessParser.Importer.Imports.Clear();
        }

        /// <summary>
        /// Returns an <see cref="IPathResolver"/> instance used by the specified LESS lessParser.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        private IPathResolver GetPathResolver(Parser lessParser)
        {
            var importer = lessParser.Importer as Importer;
            var fileReader = importer.FileReader as VirtualFileReader;

            return fileReader.PathResolver;
        }

        /// <summary>
        /// Informs the LESS parser about the path to the currently processed file. 
        /// This is done by using a custom <see cref="IPathResolver"/> implementation.
        /// </summary>
        /// <param name="lessParser">The LESS parser.</param>
        /// <param name="currentFilePath">The path to the currently processed file.</param>
        private void SetCurrentFilePath(Parser lessParser, string currentFilePath)
        {
            var importer = lessParser.Importer as Importer;

            if (importer == null)
                throw new InvalidOperationException("Unexpected dotless importer type.");

            var fileReader = importer.FileReader as VirtualFileReader;

            if (fileReader == null)
            {
                importer.FileReader = new VirtualFileReader(new VirtualPathResolver(currentFilePath));
            }
            else
            {
                var pathResolver = fileReader.PathResolver as VirtualPathResolver;

                if (pathResolver == null)
                {
                    fileReader.PathResolver = new VirtualPathResolver(currentFilePath);
                }
                else
                {
                    pathResolver.CurrentFilePath = currentFilePath;
                }
            }
        }
    }

    public class ThemeTransformer : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            const string ThemeName = "cosmo";
            const string VariablesAppendFormat = "@import \"{0}\variables.less\";\n";
            const string BootswatchAppendFormat = "\n@import \"{0}\bootswatch.less\";";
            string content = string.Concat(
                string.Format(VariablesAppendFormat, ThemeName), 
                response.Content, 
                string.Format(BootswatchAppendFormat, ThemeName));

            response.Content = content;
        }
    }
}