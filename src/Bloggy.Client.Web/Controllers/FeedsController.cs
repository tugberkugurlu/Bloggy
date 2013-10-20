using Bloggy.Client.Web.Infrastructure;
using Bloggy.Client.Web.Infrastructure.ActionResults;
using Bloggy.Client.Web.Infrastructure.Logging;
using Bloggy.Domain.Entities;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Controllers
{
    public class FeedsController : RavenController
    {
        private const string BlogPostHostUri = "http://www.tugberkugurlu.com/";
        private const string BlogPostUriFormat = BlogPostHostUri + "archive/{0}";
        private readonly IAsyncDocumentSession _documentSession;

        public FeedsController(IMvcLogger logger, IAsyncDocumentSession documentSession) : base(logger)
        {
            _documentSession = documentSession;
        }

        [OutputCache(Duration = 900, VaryByParam = "none")]
        public async Task<RssFeedResult> Rss()
        {
            IEnumerable<BlogPost> blogPosts = await _documentSession.Query<BlogPost>()
                .OrderByDescending(post => post.CreatedOn)
                .Where(post => post.IsApproved == true)
                .Take(20)
                .ToListAsync();

            IEnumerable<SyndicationItem> syndicationItems = blogPosts.Where(post => post.DefaultSlug != null).Select(post =>
                new SyndicationItem(post.Title, post.Content, new Uri(string.Format(BlogPostUriFormat, post.DefaultSlug.Path)), post.Id, post.CreatedOn));

            SyndicationFeed feed = new SyndicationFeed("Tugberk Ugurlu's Blog", null, new Uri(BlogPostHostUri), syndicationItems);

            return new RssFeedResult(feed);
        }
    }
}