using System;

namespace Bloggy.Domain.Entities
{
    public interface IComment
    {
        string Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Url { get; set; }
        string AuthProvider { get; set; }
        string Content { get; set; }
        string GravatarHash { get; }
        bool IsByAuthor { get; set; }
        bool IsSpam { get; set; }
        bool IsApproved { get; set; }
    }
}
