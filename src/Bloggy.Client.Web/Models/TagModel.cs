
using System;
namespace Bloggy.Client.Web.Models
{
    public class TagModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Count { get; set; }
        public DateTimeOffset LastSeenAt { get; set; }
    }
}