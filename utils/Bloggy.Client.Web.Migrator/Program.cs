using System.Threading.Tasks;
using Bloggy.Client.Web.Infrastructure.Managers;
using Bloggy.Domain.Entities;
using Bloggy.Domain.Indexes;
using Bloggy.Wrappers.Akismet;
using Bloggy.Wrappers.Akismet.RequestModels;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Bloggy.Client.Web.Migrator
{
    class Program
    {
        private static readonly IConfigurationManager ConfigManager = new FallbackEnabledConfigurationManager();

        static void Main(string[] args)
        {
            AkismetCredentials akismetCreds = RetrieveAkismetCredentials();
            AkismetClient akismetClient = new AkismetClient(akismetCreds.ApiKey, akismetCreds.Blog);
            Console.WriteLine(@"Retrieving old blog posts.");
            IEnumerable<BlogPost> blogs = RetrieveOldPosts();

            using (IDocumentStore store = RetrieveDocumentStore())
            using (IDocumentSession ses = store.OpenSession())
            {
                foreach (BlogPost blogPost in blogs)
                {
                    ses.Store(blogPost);

                    Console.WriteLine(@"Retrieving comments for blog post '{0}'.", blogPost.SecondaryId.Value);
                    IEnumerable<BlogPostComment> comments = RetrieveComments(blogPost.SecondaryId.Value);

                    foreach (BlogPostComment comment in comments)
                    {
                        bool isSpam = CheckAgainstSpamAsync(akismetClient, comment, blogPost.DefaultSlug.Path).Result;
                        comment.IsSpam = isSpam;
                        comment.IsApproved = !isSpam;
                        comment.BlogPostId = blogPost.Id;
                        ses.Store(comment);

                        // add CommentId to blogPost CommentIds list
                        blogPost.CommentIds.Add(comment.Id);
                    }
                }

                Console.WriteLine(@"Saving all changes...");
                ses.SaveChanges();
            }

            Console.WriteLine(@"Done");
            Console.ReadLine();
        }

        private static AkismetCredentials RetrieveAkismetCredentials()
        {
            string apiKey = ConfigManager.AkismetApiKey;
            string blog = ConfigManager.AkismetBlog;

            return new AkismetCredentials(apiKey, blog);
        }

        private static string RetrieveOldBlogConnectionString()
        {
            string connStr = Environment.GetEnvironmentVariable("Bloggy:OldBlog:ConnectionString", EnvironmentVariableTarget.User);
            if (connStr == null)
            {
                throw new NotSupportedException("Bloggy:OldBlog:ConnectionString user environment variable is not set.");
            }

            return connStr;
        }

        private static IEnumerable<BlogPost> RetrieveOldPosts()
        {
            string connectionString = RetrieveOldBlogConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Blogs WHERE Published = 1", connection))
            {
                cmd.CommandType = CommandType.Text;
                connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    IEnumerable<BlogPost> blogs = reader.Select(r => BlogBuilder(r)).ToList();
                    return blogs;
                }
            }
        }

        private static IEnumerable<BlogPostComment> RetrieveComments(int blogPostId)
        {
            string connectionString = RetrieveOldBlogConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.BlogsComments WHERE BLogID = " + blogPostId.ToString(CultureInfo.InvariantCulture), connection))
            {
                cmd.CommandType = CommandType.Text;
                connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    IEnumerable<BlogPostComment> comments = reader.Select(r => CommentBuilder(r)).ToList();
                    return comments;
                }
            }
        }

        private static async Task<bool> CheckAgainstSpamAsync(AkismetClient akismetClient, BlogPostComment comment, string slug)
        {
            bool isSpam = true;
            try
            {
                Console.WriteLine(@"Checing comment '{0}' against spam.", comment.Name);
                AkismetResponse<bool> akismetResponse = await akismetClient.CheckCommentAsync(new AkismetCommentRequestModel
                {
                    CommentAuthor = comment.Name,
                    CommentAuthorEmail = comment.Email,
                    CommentAuthorUrl = comment.Url,
                    CommentContent = comment.Content,
                    CommentType = "Comment",
                    Permalink = "http://www.tugberkugurlu/archive/" + slug,
                    UserIp = comment.CreationIp,
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6"

                });

                if (akismetResponse.IsSuccessStatusCode)
                {
                    isSpam = akismetResponse.Entity;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return isSpam;
        }

        private static BlogPost BlogBuilder(SqlDataReader reader)
        {
            return new BlogPost
            {
                SecondaryId = int.Parse(reader["BlogID"].ToString()),
                Title = reader["ArticleTitle"].ToString(),
                BriefInfo = reader["BriefInformation"].ToString(),
                Content = reader["BlogArticle"].ToString(),
                IsApproved = bool.Parse(reader["Published"].ToString()),
                CreatedOn = DateTimeOffset.Parse(reader["CreationDate"].ToString()),
                CreationIp = reader["CreationIp"].ToString(),
                AllowComments = true,
                Language = "en-US",
                LastUpdatedOn = DateTimeOffset.Now,
                LastUpdateIp = "::1",

                Slugs = new Collection<Slug>(new List<Slug> 
                    {
                        new Slug { 
                            Path = BlogUrlGeneration.GenerateUrl(reader["ArticleTitle"].ToString()), 
                            CreatedOn = DateTimeOffset.Now,
                            IsDefault = true
                        } 
                    }),

                Tags = reader["Tags"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(tag => new Tag
                    {
                        Name = tag.Trim(),
                        Slug = tag.Trim().ToSlug()
                    }).ToList(),
            };
        }

        private static BlogPostComment CommentBuilder(SqlDataReader reader)
        {
            return new BlogPostComment
            {
                Content = reader["CommentText"].ToString(),
                CreatedOn = DateTimeOffset.Parse(reader["CommentDate"].ToString()),
                CreationIp = reader["CreationIp"].ToString(),
                Email = reader["CommenterEmail"].ToString(),
                IsApproved = bool.Parse(reader["Approved"].ToString()),
                IsByAuthor = bool.Parse(reader["IsAuthorOfThisPost"].ToString()),
                LastUpdatedOn = DateTimeOffset.Now,
                LastUpdateIp = "::1",
                Name = reader["CoommenterName"].ToString(),
                Url = reader["CommenterWebSite"].ToString()
            };
        }

        private static IDocumentStore RetrieveDocumentStore()
        {
            IDocumentStore store = new DocumentStore
            {
                Url = ConfigManager.RavenDbUrl,
                DefaultDatabase = ConfigManager.RavenDbDefaultDatabase
            }.Initialize();

            store.DatabaseCommands.EnsureDatabaseExists(ConfigManager.RavenDbDefaultDatabase);
            IndexCreation.CreateIndexes(typeof(Tags_Count).Assembly, store);

            return store;
        }

        private struct AkismetCredentials
        {
            public AkismetCredentials(string apiKey, string blog)
                : this()
            {
                ApiKey = apiKey;
                Blog = blog;
            }

            public string ApiKey { get; private set; }
            public string Blog { get; private set; }
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> Select<T>(this SqlDataReader reader, Func<SqlDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }
    }

    public static class BlogUrlGeneration
    {
        public static string GenerateUrl(string phrase)
        {
            string value = phrase.ToLower();
            value = value.Replace(" / ", "-");
            value = value.Replace(" - ", "-");
            value = value.Replace("=\"", "-");
            value = value.Replace(" = ", "-");
            value = value.Replace("=", "-");
            value = value.Replace(" \" ", "-");
            value = value.Replace(" \"", "-");
            value = value.Replace("\" ", "-");
            value = value.Replace(" . ", "-");
            value = value.Replace(" .", "");
            value = value.Replace(". ", "-");
            value = value.Replace(" ? ", "-");
            value = value.Replace(" ?", "");
            value = value.Replace(" ! ", "-");
            value = value.Replace(" !", "");
            value = value.Replace(" | ", "-");
            value = value.Replace(" |", "");
            value = value.Replace(" > ", "-");
            value = value.Replace(" >", "");
            value = value.Replace(" < ", "-");
            value = value.Replace(" <", "");
            value = value.Replace(" ", "-");
            value = value.Replace(",", "");
            value = value.Replace("|", "-");
            value = value.Replace("&&", "");
            value = value.Replace("&", "and");
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            value = value.Replace("!", "");
            value = value.Replace("'", "-");
            value = value.Replace("\"", "-");
            value = value.Replace("\\", "-");
            value = value.Replace("/", "-");
            value = value.Replace(":", "-");
            value = value.Replace(".", "-");
            value = value.Replace("c#", "c-sharp");
            value = value.Replace("#", "sharp");
            value = value.Replace("{", "");
            value = value.Replace("}", "");
            value = value.Replace("[", "");
            value = value.Replace("]", "");
            value = value.Replace("?", "-");
            value = value.Replace("*", "");
            value = value.Replace(";", "");
            value = value.Replace("ı", "i");
            value = value.Replace("ü", "u");
            value = value.Replace("ö", "o");
            value = value.Replace("ğ", "g");
            value = value.Replace("ç", "c");
            value = value.Replace("ş", "s");
            string[] trimmedChars = new string[]
			{
				"-",
				" ",
				"...",
				"."
			};
            for (int i = 0; i < trimmedChars.Length; i++)
            {
                string v = trimmedChars[i].ToString();
                if (value.StartsWith(v))
                {
                    value = value.TrimStart(new char[]
					{
						char.Parse(v)
					});
                }
                if (value.EndsWith(v))
                {
                    value = value.TrimEnd(new char[]
					{
						char.Parse(v)
					});
                }
            }
            return value.Trim();
        }
    }
}