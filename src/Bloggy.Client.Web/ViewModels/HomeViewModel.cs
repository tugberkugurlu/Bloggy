using Bloggy.Client.Web.Models;
using Bloggy.Domain;

namespace Bloggy.Client.Web.ViewModels
{
    public class HomeViewModel
    {
        public PaginatedList<BlogPostModelLight> BlogPosts { get; set; }
    }
}