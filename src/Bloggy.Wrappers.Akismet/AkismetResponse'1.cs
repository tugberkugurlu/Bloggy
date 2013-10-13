using System.Net;

namespace Bloggy.Wrappers.Akismet
{
    public class AkismetResponse<T> : AkismetResponse
    {
        public AkismetResponse(HttpStatusCode status) : base(status)
        {
        }

        public AkismetResponse(HttpStatusCode status, T entity) : base(status)
        {
            Entity = entity;
        }

        public T Entity { get; private set; }
    }
}
