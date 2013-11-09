using Bloggy.Wrappers.Akismet.RequestModels;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Bloggy.Wrappers.Akismet.Tests.Integration
{
    public class AkismetClientTests
    {
        [Fact]
        public async Task CheckCommentAsyncShouldReturnOkForTheComment()
        {
            // Arrange
            AkismetCredentials akismetCreds = RetrieveAkismetCredentials();
            using (AkismetClient akismetClient = new AkismetClient(akismetCreds.ApiKey, akismetCreds.Blog))
            {
                AkismetCommentRequestModel requestModel = new AkismetCommentRequestModel
                {
                    UserIp = "127.0.0.1",
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6",
                    Referrer = "http://www.google.com",
                    Permalink = string.Concat(akismetCreds.Blog, "blog/post=1"),
                    CommentType = "comment",
                    CommentAuthor = "Tugberk",
                    CommentAuthorEmail = "tugberk@tugberkugurlu.com",
                    CommentAuthorUrl = "http://tugberk.me",
                    CommentContent = "What do you mean by this? How can we integrate this into our pojects?"
                };

                // Act
                AkismetResponse<bool> response = await akismetClient.CheckCommentAsync(requestModel);

                // Assert
                Assert.Equal(true, response.IsSuccessStatusCode);
                Assert.Equal(false, response.Entity);
            }
        }

        [Fact]
        public async Task CheckCommentAsyncShouldReturnSpamForASpamComment()
        {
            // Arrange
            AkismetCredentials akismetCreds = RetrieveAkismetCredentials();
            using (AkismetClient akismetClient = new AkismetClient(akismetCreds.ApiKey, akismetCreds.Blog))
            {
                AkismetCommentRequestModel requestModel = new AkismetCommentRequestModel
                {
                    UserIp = "127.0.0.1",
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6",
                    Referrer = "http://www.google.com/",
                    Permalink = string.Concat(akismetCreds.Blog, "blog/post=1"),
                    CommentType = "comment",
                    CommentAuthor = "best viagra site",
                    CommentAuthorEmail = "theradkes@sbcglobal.net",
                    CommentAuthorUrl = "http://bestedpillsonline.net/",
                    CommentContent = "That's an ingenious way of thinking about it."
                };

                // Act
                AkismetResponse<bool> response = await akismetClient.CheckCommentAsync(requestModel);

                // Assert
                Assert.Equal(true, response.IsSuccessStatusCode);
                Assert.Equal(true, response.Entity);
            }
        }

        // privates

        private static AkismetCredentials RetrieveAkismetCredentials()
        {
            string apiKey = Environment.GetEnvironmentVariable("Bloggy:Akismet:ApiKey", EnvironmentVariableTarget.User);
            string blog = Environment.GetEnvironmentVariable("Bloggy:Akismet:Blog", EnvironmentVariableTarget.User);

            if (apiKey == null || blog == null)
            {
                throw new NotSupportedException("Either Bloggy:Akismet:ApiKey or Bloggy:Akismet:Blog user environment variable is not set.");
            }

            return new AkismetCredentials(apiKey, blog);
        }

        private struct AkismetCredentials
        {
            public AkismetCredentials(string apiKey, string blog) : this()
            {
                ApiKey = apiKey;
                Blog = blog;
            }

            public string ApiKey { get; private set; }
            public string Blog { get; private set; }
        }
    }
}