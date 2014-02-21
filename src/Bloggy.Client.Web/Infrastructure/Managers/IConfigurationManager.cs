namespace Bloggy.Client.Web.Infrastructure.Managers
{
    public interface IConfigurationManager
    {
        string OldBlogConnectionString { get;  }

        bool IsAkismetEnabled { get; }
        string AkismetApiKey { get; }
        string AkismetBlog { get; }

        bool IsRecaptchaEnabled { get; }
        string RecaptchaPublicKey { get; }
        string RecaptchaPrivateKey { get; }

        string RavenDbUrl { get; }
        string RavenDbDefaultDatabase { get; }

        string AzureBlobStorageConnectionString { get; }
    }
}