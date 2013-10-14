using Bloggy.Wrappers.Akismet.RequestModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
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
            string apiKey = ConfigurationManager.AppSettings["akismet:apiKey"];
            string blog = ConfigurationManager.AppSettings["akismet:blog"];
            using (AkismetClient akismetClient = new AkismetClient(apiKey, blog))
            {
                CommentRequestModel requestModel = new CommentRequestModel
                {
                    UserIp = "127.0.0.1",
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6",
                    Referrer = "http://www.google.com",
                    Permalink = "http://www.tugberkugurlu.com/blog/post=1",
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
            string apiKey = ConfigurationManager.AppSettings["akismet:apiKey"];
            string blog = ConfigurationManager.AppSettings["akismet:blog"];
            using (AkismetClient akismetClient = new AkismetClient(apiKey, blog))
            {
                CommentRequestModel requestModel = new CommentRequestModel
                {
                    UserIp = "127.0.0.1",
                    UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6",
                    Referrer = "http://www.google.com/",
                    Permalink = "http://yourblogdomainname.com/blog/post=1",
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
    }
}