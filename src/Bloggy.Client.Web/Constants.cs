
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
}