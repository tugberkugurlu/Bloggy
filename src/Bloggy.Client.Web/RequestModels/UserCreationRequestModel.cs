using System.ComponentModel.DataAnnotations;

namespace Bloggy.Client.Web.RequestModels
{
    public class UserCreationRequestModel : BaseUserRequestModel
    {
        [Required]
        [EmailAddress]
        [StringLength(300)]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "EmailAddress")]
        public string Email { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "PasswordConfirm")]
        public string PasswordConfirm { get; set; }
    }
}