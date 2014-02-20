﻿using System;

namespace Bloggy.Client.Web.Infrastructure.AtomPub
{
    /// <summary>
    /// An interface for commands that can create or update publications.
    /// </summary>
    public interface IPublicationCommand
    {
        /// <summary>
        /// The title of the publication.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// A short description of the content.
        /// </summary>
        string Summary { get; set; }

        /// <summary>
        /// The publication content.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// The publication content type e.g. 'text' or 'html'.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// An optional publish date for the entry.
        /// </summary>
        DateTimeOffset? PublishDate { get; set; }

        /// <summary>
        /// A string array of categories related to the content.
        /// </summary>
        string[] Categories { get; set; }
    }
}