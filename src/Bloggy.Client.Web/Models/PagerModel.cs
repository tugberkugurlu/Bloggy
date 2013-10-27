using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggy.Client.Web.Models
{
    public class PagerModel
    {
        public int CurrentPage { get; set; }
        public bool IsOlderDisabled { get; set; }
        public bool IsNewerDisabled { get; set; }
    }
}