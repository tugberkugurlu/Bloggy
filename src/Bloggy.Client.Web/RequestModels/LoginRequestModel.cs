using System.ComponentModel.DataAnnotations;

namespace Bloggy.Client.Web.RequestModels
{
    public class LoginRequestModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}