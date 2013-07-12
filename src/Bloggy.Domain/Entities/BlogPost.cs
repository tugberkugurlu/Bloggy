using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloggy.Domain.Entities
{
    public class BlogPost
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }
        public string BriefInfo  { get; set; }
        public string Content { get; set; }
        public ICollection<string> Tags { get; set; }
        public string IsApproved { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreationIp { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}
