﻿using System;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime LastUpdated { get; set; }

        public Post()
        {
            LastUpdated = DateTime.UtcNow;
        }
    }
}