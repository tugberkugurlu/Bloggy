using System.Collections.Generic;
using System.Net.Http;

namespace Bloggy.Wrappers.Akismet.RequestModels
{
    internal static class CommentRequestModelExtensions
    {
        internal static FormUrlEncodedContent ToFormUrlEncodedContent(this AkismetCommentRequestModel requestModel, string blog)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CommentConstants.BlogRequestParamName, blog),
                new KeyValuePair<string, string>(CommentConstants.CommentAuthorRequestParamName, requestModel.CommentAuthor),
                new KeyValuePair<string, string>(CommentConstants.CommentAuthorEmailRequestParamName, requestModel.CommentAuthorEmail),
                new KeyValuePair<string, string>(CommentConstants.CommentAuthorUrlRequestParamName, requestModel.CommentAuthorUrl),
                new KeyValuePair<string, string>(CommentConstants.CommentContentRequestParamName, requestModel.CommentContent),
                new KeyValuePair<string, string>(CommentConstants.CommentTypeRequestParamName, requestModel.CommentType),
                new KeyValuePair<string, string>(CommentConstants.PermalinkRequestParamName, requestModel.Permalink),
                new KeyValuePair<string, string>(CommentConstants.ReferrerRequestParamName, requestModel.Referrer),
                new KeyValuePair<string, string>(CommentConstants.UserAgentRequestParamName, requestModel.UserAgent),
                new KeyValuePair<string, string>(CommentConstants.UserIpRequestParamName, requestModel.UserIp)
            });

            return content;
        }
    }
}
