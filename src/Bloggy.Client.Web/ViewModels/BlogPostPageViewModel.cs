using Bloggy.Client.Web.Models;
using Bloggy.Client.Web.RequestModels;
using System.Collections.Generic;

namespace Bloggy.Client.Web.ViewModels
{
    public class BlogPostPageViewModel
    {
        public BlogPostPageViewModel()
        {
            CommentPostRequestModel = new CommentPostRequestModel();
        }

        public BlogPostModelLight BlogPost { get; set; }
        public IEnumerable<BlogPostCommentModel> Comments { get; set; }
        public CommentPostRequestModel CommentPostRequestModel { get; set; }
    }
}