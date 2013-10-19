using Bloggy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloggy.Domain.Managers
{
    public interface IBlogManager
    {
        Task<BlogPost> GetBlogPostAsync(string id);
        Task<BlogPost> GetBlogPostBySlugAsync(string slug);
    }
}