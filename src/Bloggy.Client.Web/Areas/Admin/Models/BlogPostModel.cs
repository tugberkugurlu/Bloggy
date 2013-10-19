using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggy.Client.Web.Areas.Admin.Models
{
    public class BlogPostModel
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }
        public string BriefInfo { get; set; }
        public string Content { get; set; }

        public string[] Tags { get; set; }
        public string[] Slugs { get; set; }

        public bool AllowComments { get; set; }
        public bool IsApproved { get; set; }
    }
}