using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggy.Client.Web.Models
{
    public class LanguageModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Culture { get; set; }
        public byte Order { get; set; }
    }
}