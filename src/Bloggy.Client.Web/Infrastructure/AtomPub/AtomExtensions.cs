using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System;
using System.Linq;
using System.ServiceModel.Syndication;

namespace Bloggy.Client.Web.Infrastructure.AtomPub
{
    public static class AtomExtensions
    {
        public static SyndicationFeed Syndicate(this IPublicationFeed feed)
        {
            SyndicationFeed atomFeed = new SyndicationFeed
            {
                Title = new TextSyndicationContent(feed.Title),
                Items = feed.Items.Select(item => item.Syndicate()),
                Description = new TextSyndicationContent(feed.Summary ?? string.Empty)
            };

            atomFeed.Authors.Add(new SyndicationPerson { Name = feed.Author });

            foreach (Link link in feed.Links)
            {
                atomFeed.Links.Add(new SyndicationLink(new Uri(link.Href)) { RelationshipType = link.Rel, Title = link.Title });
            }

            return atomFeed;
        }

        public static SyndicationItem Syndicate(this IPublication publication)
        {
            SyndicationItem item = new SyndicationItem
            {
                Id = publication.Id,
                Title = new TextSyndicationContent(publication.Title, TextSyndicationContentKind.Plaintext),
                LastUpdatedTime = publication.PublishDate ?? publication.LastUpdated, // use publish date if it exists (for posts)
                Summary = new TextSyndicationContent(publication.Summary, TextSyndicationContentKind.Plaintext),
                Content = GetSyndicationContent(publication.Content, publication.ContentType)
            };

            // Optional according to Atom spec
            if (publication.PublishDate.HasValue)
            {
                item.PublishDate = publication.PublishDate.Value;
            }

            foreach (IPublicationCategory category in publication.Categories)
            {
                item.Categories.Add(new SyndicationCategory(category.Name, publication.CategoriesScheme, category.Label));
            }

            foreach (Link link in publication.Links)
            {
                item.Links.Add(new SyndicationLink(new Uri(link.Href)) { RelationshipType = link.Rel, Title = link.Title });
            }

            return item;
        }

        public static SyndicationItem Syndicate(this IPublicationMedia publicationMedia)
        {
            SyndicationItem item = new SyndicationItem
            {
                Id = publicationMedia.Id,
                Title = new TextSyndicationContent(publicationMedia.Title, TextSyndicationContentKind.Plaintext),
                LastUpdatedTime = publicationMedia.LastUpdated,
                Summary = new TextSyndicationContent(publicationMedia.Summary, TextSyndicationContentKind.Plaintext),
                Content = SyndicationContent.CreateUrlContent(publicationMedia.ImageUrl, publicationMedia.ContentType),
            };

            foreach (Link link in publicationMedia.Links)
            {
                item.Links.Add(new SyndicationLink(new Uri(link.Href)) { RelationshipType = link.Rel, Title = link.Title });
            }

            return item;
        }

        public static void ReadSyndicationItem<TCommand>(this TCommand command, SyndicationItem item) where TCommand : IPublicationCommand
        {
            command.Title = item.Title.Text;
            command.Summary = item.Summary != null ? item.Summary.Text : null;
            command.Content = ((TextSyndicationContent)item.Content).Text;
            command.ContentType = item.Content.Type;
            command.Categories = item.Categories.Select(c => c.Name).ToArray();
            command.PublishDate = GetPublishDate(item.PublishDate);
        }

        private static SyndicationContent GetSyndicationContent(string content, string contentType)
        {
            if (string.IsNullOrEmpty(content) || contentType.ToLowerInvariant() == "text")
            {
                return SyndicationContent.CreatePlaintextContent(content ?? string.Empty);
            }

            return SyndicationContent.CreateHtmlContent(content);
        }

        public static DateTime GetPublishDate(DateTimeOffset syndicationDate)
        {
            DateTime publishDate = syndicationDate.UtcDateTime;

            // if the publish date has not been set it will be equal to DateTime.MinValue.
            // So, set it to DateTime.UtcNow.
            return publishDate == DateTime.MinValue ? DateTime.UtcNow : publishDate;
        }
    }
}