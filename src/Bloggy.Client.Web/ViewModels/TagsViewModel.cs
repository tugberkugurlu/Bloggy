using Bloggy.Client.Web.Models;
using System.Collections.Generic;

namespace Bloggy.Client.Web.ViewModels
{
    public class TagsViewModel
    {
        public IEnumerable<TagModel> Tags { get; set; }
    }
}