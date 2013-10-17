
namespace Bloggy.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public Author Author { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string TwitterHandle { get; set; }
    }
}