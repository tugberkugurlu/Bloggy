using System;

namespace Bloggy.Client.Web.Infrastructure.Managers
{
    public sealed class FallbackEnabledConfigurationManager : DefaultConfigurationManager
    {
        private const string NeedsToBeDefinedErrorFormat = "'{0}' value needs to be defined either inside the config file or as a user environment variable.";
        private readonly string _akismetApiKey;
        private readonly string _akismetBlog;
        private readonly string _oldBlogConnectionString;

        public FallbackEnabledConfigurationManager()
        {
            _akismetApiKey = GetValue(AkismetApiKeyConfigKey, base.AkismetApiKey);
            _akismetBlog = GetValue(AkismetBlogConfigKey, base.AkismetBlog);
            _oldBlogConnectionString = GetValue(OldBlogConnectionStringConfigKey, base.OldBlogConnectionString);
        }

        public override string OldBlogConnectionString
        {
            get { return _oldBlogConnectionString; }
        }

        public override string AkismetApiKey
        {
            get { return _akismetApiKey; }
        }

        public override string AkismetBlog
        {
            get { return _akismetBlog; }
        }

        // privates

        private string GetValue(string key, string baseValue)
        {
            if (string.IsNullOrWhiteSpace(baseValue))
            {
                string envVariableValue = GetEnvVariableValue(key);
                if (string.IsNullOrWhiteSpace(envVariableValue))
                {
                    throw new NotSupportedException(string.Format(NeedsToBeDefinedErrorFormat, key));
                }

                return envVariableValue;
            }

            return baseValue;
        }

        private string GetEnvVariableValue(string key)
        {
            return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
        }
    }
}