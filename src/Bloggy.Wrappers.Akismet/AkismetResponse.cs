using System.Net;

namespace Bloggy.Wrappers.Akismet
{
    public class AkismetResponse
    {
        public AkismetResponse(HttpStatusCode status) 
        {
            Status = status;
        }

        public HttpStatusCode Status { get; private set; }

        public string ErrorMessage { get; set; }

        public bool IsSuccessStatusCode
        {
            get
            {
                return Status >= HttpStatusCode.OK && Status <= (HttpStatusCode)299;
            }
        }
    }
}