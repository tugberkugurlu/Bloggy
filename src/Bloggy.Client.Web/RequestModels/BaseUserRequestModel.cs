using System.ComponentModel.DataAnnotations;

namespace Bloggy.Client.Web.RequestModels
{
    public abstract class BaseUserRequestModel
    {
        [Required]
        [StringLength(50)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(AttributeResosurces), Name = "Password")]
        public string Password { get; set; }
    }
}