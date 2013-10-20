using System;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Bloggy.Client.Web.Infrastructure.ActionResults
{
    /// <remarks>
    /// Refer to https://github.com/benfoster/Fabrik.Common/blob/dev/src/Fabrik.Common.Web/ActionResults/FeedResult.cs.
    /// </remarks>    
    public class FeedResult : ActionResult
    {
        public SyndicationFeedFormatter Formatter { get; private set; }
        public string ContentType { get; private set; }
        public Encoding Encoding { get; set; }

        public FeedResult(SyndicationFeedFormatter formatter, string contentType)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            Formatter = formatter;
            ContentType = contentType;
            Encoding = Encoding.UTF8;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = ContentType;
            response.ContentEncoding = Encoding;

            using (XmlTextWriter writer = new XmlTextWriter(response.Output))
            {
                writer.Formatting = Formatting.Indented;
                Formatter.WriteTo(writer);
            }
        }
    }
}