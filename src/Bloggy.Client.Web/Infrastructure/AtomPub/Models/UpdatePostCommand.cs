﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Bloggy.Client.Web.Infrastructure.AtomPub.Models
{
    public class UpdatePostCommand : IPublicationCommand
    {
        [Required]
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public DateTime? PublishDate { get; set; }

        public UpdatePostCommand()
        {
            PublishDate = DateTime.UtcNow;
            Tags = new string[0];
        }

        string[] IPublicationCommand.Categories
        {
            get { return Tags; }
            set { Tags = value; }
        }
    }
}