using Bloggy.Client.Web.Models;
using System.Collections.Generic;

namespace Bloggy.Client.Web.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<BlogPostModelLight> BlogPosts { get; set; }
    }
}