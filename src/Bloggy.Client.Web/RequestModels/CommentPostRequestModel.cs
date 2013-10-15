using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Bloggy.Client.Web.RequestModels
{
    public class CommentPostRequestModel
    {
        [Required]
        [StringLength(50)]
        public string CommentAuthorName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(300)]
        public string CommentAuthorEmail { get; set; }

        [StringLength(300)]
        public string CommentAuthorUrl { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string CommentContent { get; set; }
    }
}