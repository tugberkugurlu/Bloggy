using Raven.Imports.Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Bloggy.Domain.Entities
{
    public class BlogPostComment : IComment, ITrackable
    {
        public string Id { get; set; }
        public string BlogPostId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string AuthProvider { get; set; }
        public string Content { get; set; }
        public bool IsByAuthor { get; set; }
        public bool IsSpam { get; set; }
        public bool IsApproved { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public string CreationIp { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
        public string LastUpdateIp { get; set; }

        [JsonIgnore]
        public string GravatarHash
        {
            get
            {
                return Email.ToGravatarHash();
            }
        }
    }
}
