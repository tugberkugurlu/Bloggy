using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Formatters
{
    public class AtomPubCategoryMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private const string AtomCategoryMediaType = "application/atomcat+xml";

        public AtomPubCategoryMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(AtomCategoryMediaType));
            this.AddQueryStringMapping("format", "atomcat", AtomCategoryMediaType);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(IPublicationCategoriesDocument).IsAssignableFrom(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            IPublicationCategoriesDocument document = value as IPublicationCategoriesDocument;

            InlineCategoriesDocument atomDoc = new InlineCategoriesDocument(
                document.Categories.Select(c => new SyndicationCategory(c.Name) { Label = c.Label }),
                document.IsFixed,
                document.Scheme
            );

            AtomPub10CategoriesDocumentFormatter formatter = new AtomPub10CategoriesDocumentFormatter(atomDoc);

            using (writeStream)
            {
                using (var writer = XmlWriter.Create(writeStream))
                {
                    formatter.WriteTo(writer);
                }
            }
        }
    }
}