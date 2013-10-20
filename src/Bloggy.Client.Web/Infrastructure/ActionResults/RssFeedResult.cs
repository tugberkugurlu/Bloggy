using System.ServiceModel.Syndication;

namespace Bloggy.Client.Web.Infrastructure.ActionResults
{
    /// <summary>
    /// An ActionResult for returning RSS feeds.
    /// </summary>
    /// <remarks>
    /// Refer to https://github.com/benfoster/Fabrik.Common/blob/dev/src/Fabrik.Common.Web/ActionResults/RssFeedResult.cs.
    /// </remarks>
    public class RssFeedResult : FeedResult
    {
        public RssFeedResult(SyndicationFeed feed)
            : base(new Rss20FeedFormatter(feed), "application/rss+xml")
        {
        }
    }
}