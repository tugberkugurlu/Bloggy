using Bloggy.Client.Web.Infrastructure.Validation;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Bloggy.Client.Web.Areas.Admin.RequestModels
{
    public class BlogPostRequestModel
    {
        [Required]
        [Options("en-US")]
        public string Language { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; }

        public bool IsApproved { get; set; }

        public string[] Tags { get; set; }

        [Required]
        [StringLength(250)]
        public string BriefInfo { get; set; }

        [Required]
        [AllowHtml]
        [UIHint("tinymce_full")]
        public string Content { get; set; }
    }
}