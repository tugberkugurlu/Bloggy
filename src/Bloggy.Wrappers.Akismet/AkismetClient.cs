using Bloggy.Wrappers.Akismet.RequestModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bloggy.Wrappers.Akismet
{
    public sealed class AkismetClient : IDisposable
    {
        private const string BaseApiUri = "rest.akismet.com";
        private const string BaseApiUriPath = "/1.1";
        private readonly HttpClient _httpClient;
        private readonly string _blog;
        private bool _disposed;

        public AkismetClient(string apiKey, string blog) 
            : this(apiKey, blog, new HttpClientHandler())
        {
        }

        public AkismetClient(string apiKey, string blog, HttpMessageHandler httpClientHandler)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException("apiKey");
            }

            if (blog == null)
            {
                throw new ArgumentNullException("blog");
            }

            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.BaseAddress = new Uri(string.Format("https://{0}.{1}", apiKey, BaseApiUri));

            _httpClient = httpClient;
            _blog = blog;
        }

        public async Task<AkismetResponse<bool>> CheckCommentAsync(AkismetCommentRequestModel commentRequestModel)
        {
            if (commentRequestModel == null)
            {
                throw new ArgumentNullException("commentRequestModel");
            }

            string requestUri = string.Concat(BaseApiUriPath, "/comment-check");
            using (HttpContent content = commentRequestModel.ToFormUrlEncodedContent(_blog))
            using (HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content))
            {
                AkismetResponse<bool> result;
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    bool responseResult;
                    if (bool.TryParse(responseContent, out responseResult))
                    {
                        result = new AkismetResponse<bool>(response.StatusCode, responseResult);
                    }
                    else
                    {
                        string errorMessageFormat = "Couldn't cast the response result into Boolean! Status Code: {0}, Message Body: {1}";
                        throw new InvalidOperationException(string.Format(errorMessageFormat, response.StatusCode, responseContent));
                    }
                }
                else
                {
                    result = new AkismetResponse<bool>(response.StatusCode);
                    result.ErrorMessage = responseContent;
                }

                return result;
            }
        }

        public async Task<AkismetResponse> SubmitSpamAsync(AkismetCommentRequestModel commentRequestModel)
        {
            if (commentRequestModel == null)
            {
                throw new ArgumentNullException("commentRequestModel");
            }

            string requestUri = string.Concat(BaseApiUriPath, "/submit-spam");
            using (HttpContent content = commentRequestModel.ToFormUrlEncodedContent(_blog))
            using (HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content))
            {
                AkismetResponse result;
                if (response.IsSuccessStatusCode)
                {
                    result = new AkismetResponse(response.StatusCode);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    result = new AkismetResponse(response.StatusCode)
                    {
                        ErrorMessage = responseContent
                    };
                }

                return result;
            }
        }

        public async Task<AkismetResponse> SubmitHamAsync(AkismetCommentRequestModel commentRequestModel)
        {
            if (commentRequestModel == null)
            {
                throw new ArgumentNullException("commentRequestModel");
            }

            string requestUri = string.Concat(BaseApiUriPath, "/submit-ham");
            using (HttpContent content = commentRequestModel.ToFormUrlEncodedContent(_blog))
            using (HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content))
            {
                AkismetResponse result;
                if (response.IsSuccessStatusCode)
                {
                    result = new AkismetResponse(response.StatusCode);
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    result = new AkismetResponse(response.StatusCode)
                    {
                        ErrorMessage = responseContent
                    };
                }

                return result;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                }

                _disposed = true;
            }
        }
    }
}