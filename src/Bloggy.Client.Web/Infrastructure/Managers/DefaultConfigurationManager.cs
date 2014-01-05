using System;
using System.Configuration;

namespace Bloggy.Client.Web.Infrastructure.Managers
{
    public class DefaultConfigurationManager : IConfigurationManager
    {
        private readonly string _oldBlogConnectionString;
        private readonly bool _isAkismetEnabled;
        private readonly string _akismetApiKey;
        private readonly string _akismetBlog;
        private readonly bool _isRecaptchaEnabled;
        private readonly string _recaptchaPublicKey;
        private readonly string _recaptchaPrivateKey;
        private readonly string _ravenDbUrl;
        private readonly string _ravenDbDefaultDatabase;

        protected const string OldBlogConnectionStringConfigKey = "Bloggy:OldBlog:ConnectionString";
        protected const string AkismetEnabledConfigKey = "Bloggy:Akismet:Enabled";
        protected const string AkismetApiKeyConfigKey = "Bloggy:Akismet:ApiKey";
        protected const string AkismetBlogConfigKey = "Bloggy:Akismet:Blog";
        protected const string RecaptchaActiveConfigKey = "Bloggy:Recaptcha:Active";
        protected const string RecaptchaPublicKeyConfigKey = "Bloggy:Recaptcha:PublicKey";
        protected const string RecaptchaPrivateKeyConfigKey = "Bloggy:Recaptcha:PrivateKey";
        protected const string RavenDbUrlConfigKey = "Bloggy:RavenDb:Url";
        protected const string RavenDbDefaultDatabaseConfigKey = "Bloggy:RavenDb:DefaultDatabase";

        public DefaultConfigurationManager()
        {
            Nullable<bool> isAkismetEnabled = GetValue<bool>(AkismetEnabledConfigKey);
            Nullable<bool> isRecaptchaEnabled = GetValue<bool>(RecaptchaActiveConfigKey);
            ConnectionStringSettings oldBlogConnStr = ConfigurationManager.ConnectionStrings[OldBlogConnectionStringConfigKey];

            _oldBlogConnectionString = (oldBlogConnStr != null) ? oldBlogConnStr.ConnectionString : null;
            _akismetApiKey = GetValue(AkismetApiKeyConfigKey);
            _akismetBlog = GetValue(AkismetBlogConfigKey);
            _recaptchaPrivateKey = GetValue(RecaptchaPrivateKeyConfigKey);
            _recaptchaPublicKey = GetValue(RecaptchaPublicKeyConfigKey);
            _ravenDbUrl = GetValue(RavenDbUrlConfigKey);
            _ravenDbDefaultDatabase = GetValue(RavenDbDefaultDatabaseConfigKey);
            _isAkismetEnabled = (isAkismetEnabled != null) && isAkismetEnabled.Value;
            _isRecaptchaEnabled = (isRecaptchaEnabled != null) && isRecaptchaEnabled.Value;
        }

        public virtual string OldBlogConnectionString
        {
            get { return _oldBlogConnectionString; }
        }

        public virtual bool IsAkismetEnabled
        {
            get { return _isAkismetEnabled; }
        }

        public virtual string AkismetApiKey
        {
            get { return _akismetApiKey; }
        }

        public virtual string AkismetBlog
        {
            get { return _akismetBlog; }
        }

        public virtual bool IsRecaptchaEnabled
        {
            get { return _isRecaptchaEnabled; }
        }

        public virtual string RecaptchaPublicKey
        {
            get { return _recaptchaPublicKey; }
        }

        public virtual string RecaptchaPrivateKey
        {
            get { return _recaptchaPrivateKey; }
        }

        public virtual string RavenDbUrl
        {
            get { return _ravenDbUrl; }
        }

        public virtual string RavenDbDefaultDatabase
        {
            get { return _ravenDbDefaultDatabase; }
        }

        // Privates

        private Nullable<TValue> GetValue<TValue>(string key) where TValue : struct, IConvertible
        {
            Nullable<TValue> returnValue = null;
            string valueAsString = GetValue(key);
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                returnValue = (TValue)Convert.ChangeType(valueAsString, typeof(TValue));
            }

            return returnValue;
        }

        private static string GetValue(String key)
        {
            string returnValue = null;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                returnValue = ConfigurationManager.AppSettings[key];
            }

            return returnValue;
        }
    }
}