using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Bloggy.Client.Web.RequestModels
{
    public class CommentPostRequestModel
    {
        [Required]
        [StringLength(50)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "CommentPostRequestModel_CommentAuthorName")]
        public string CommentAuthorName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(300)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "CommentPostRequestModel_CommentAuthorEmail")]
        public string CommentAuthorEmail { get; set; }

        [StringLength(300)]
        [RegularExpression(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?")]
        [DataType(DataType.Url)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "CommentPostRequestModel_CommentAuthorUrl")]
        public string CommentAuthorUrl { get; set; }

        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "CommentPostRequestModel_CommentContent")]
        public string CommentContent { get; set; }

        public string SanitizedCommentContent 
        { 
            get 
            {
                return CommentContent.ToSanitizedHtml();
            } 
        }
    }
}