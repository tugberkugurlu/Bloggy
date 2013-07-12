using Bloggy.Client.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggy.Client.Web.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<BlogPostModelLight> BlogPosts { get; set; }
    }
}