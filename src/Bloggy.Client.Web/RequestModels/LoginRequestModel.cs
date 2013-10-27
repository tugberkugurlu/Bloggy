using System.ComponentModel.DataAnnotations;

namespace Bloggy.Client.Web.RequestModels
{
    public class LoginRequestModel : BaseUserRequestModel
    {
        [Display(ResourceType = typeof(AttributeResosurces), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}