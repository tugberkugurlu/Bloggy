using Bloggy.Client.Web.Infrastructure.Hypermedia;
using System;
using System.Collections.Generic;
using System.Linq;
using Bloggy.Domain.Entities;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Models
{
    public class PostModel : Resource, IPublication
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string CategoriesScheme { get; set; }

        public PostModel()
        {
            PublishDate = DateTime.UtcNow;
        }

        public PostModel(BlogPost post, string categoriesScheme)
        {
            Id = post.Id.ToIntId();
            Title = post.Title;
            Slug = post.DefaultSlug.Path;
            Summary = post.BriefInfo;
            ContentType = "text/html";
            Content = post.Content;
            Tags = post.Tags.Select(tag => tag.Name).ToArray();
            PublishDate = post.CreatedOn;
            LastUpdated = post.LastUpdatedOn;
            CategoriesScheme = categoriesScheme;
        }

        string IPublication.Id
        {
            get
            {
                Link firstLink = Links.FirstOrDefault(link => link is SelfLink);
                return (firstLink != null) ? firstLink.Href : null;
            }
        }

        DateTimeOffset? IPublication.PublishDate
        {
            get
            {
                return PublishDate;
            }
        }

        IEnumerable<IPublicationCategory> IPublication.Categories
        {
            get
            {
                return Tags.Select(t => new PublicationCategory(t));
            }
        }
    }
}