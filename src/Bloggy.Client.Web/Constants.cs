
namespace Bloggy.Client.Web
{
    public static class Constants
    {
        public const string RecaptchaActiveAppSettingsKey = "recaptcha:active";
        public const string RecaptchaPrivateKeyAppSettingsKey = "recaptcha:privateKey";
        public const string RecaptchaPublicKeyAppSettingsKey = "recaptcha:publicKey";

        public const string AkismetActiveAppSettingsKey = "akismet:active";
        public const string AkismetApiKeyAppSettingsKey = "akismet:apiKey";
        public const string AkismetBlogAppSettingsKey = "akismet:blog";

        public const string RavenDbUrlAppSettingsKey = "ravendb:url";
        public const string RavenDbDefaultDatabaseAppSettingsKey = "ravendb:defaultDatabase";
    }

    public static class ApplicationRoles
    {
        public const string AdminRole = "Admin";
        public const string AuthorRole = "Author";
    }
}