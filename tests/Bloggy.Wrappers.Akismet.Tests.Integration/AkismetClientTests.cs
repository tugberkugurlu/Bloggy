using Bloggy.Wrappers.Akismet.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bloggy.Wrappers.Akismet.Tests.Integration
{
    public class AkismetClientTests
    {
        public async Task CheckCommentAsyncShouldReturnOkForTheComment()
        {
            // Arrange
            AkismetClient akismetClient = new AkismetClient("", "");
            CommentRequestModel requestModel = new CommentRequestModel
            {
                UserIp = "127.0.0.1",
                UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6",
                Referrer = "http://www.google.com",
                Permalink = "http://yourblogdomainname.com/blog/post=1",
                CommentType = "comment",
                CommentAuthor = "admin",
                CommentAuthorEmail = "test@test.com",
                CommentAuthorUrl = "http://www.CheckOutMyCoolSite.com",
                CommentContent = "It means a lot that you would take the time to review our software.  Thanks again."
            };

            // Act
            AkismetResponse<bool> response = await akismetClient.CheckCommentAsync(requestModel);

            // Assert
            Assert.Equal(true, response.Entity);
        }
    }
}