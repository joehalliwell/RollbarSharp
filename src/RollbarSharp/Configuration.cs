﻿using System.Configuration;
using Newtonsoft.Json;

namespace RollbarSharp
{
    public class Configuration
    {
        /// <summary>
        /// Default endpoint URL for posting notices (default: https://api.rollbar.com/api/1/item/)
        /// </summary>
        public static string DefaultEndpoint = "https://api.rollbar.com/api/1/item/";

        /// <summary>
        /// Default environment name used in notices. (default: production)
        /// </summary>
        public static string DefaultEnvironment = "production";

        /// <summary>
        /// Default language name used in notices. (default: csharp)
        /// </summary>
        public static string DefaultLanguage = "csharp";

        /// <summary>
        /// Default parameters that should be scrubbed from requests
        /// </summary>
        public static string[] DefaultScrubParams =
            new[]
                {
                    "password", "password_confirmation", "confirm_password",
                    "secret", "secret_token",
                    "creditcard", "credit_card", "credit_card_number", "card_number", "ccnum", "cc_number"
                };


        /// <summary>
        /// Encoding to use when communicating with the Rollbar endpoint (default: utf-8)
        /// </summary>
        public static string Encoding = "utf-8";


        /// <summary>
        /// URL for the Rollbar API
        /// Setting: Rollbar.Endpoint
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The server-side access token for your application in Rollbar.
        /// Also known as the post_server_item key
        ///
        /// Setting: Rollbar.AccessToken
        /// Default: None. You have to set this.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Name of the environment this app is running in. Usually "production" or "staging"
        ///
        /// Setting: Rollbar.Environment
        /// Default: production
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Platform running the code. E.g. Windows or IIS.
        ///
        /// Setting: Rollbar.Platform
        /// Default: <see cref="System.Environment.OSVersion"/>
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Code language. Defaults to csharp.
        ///
        /// Setting: Rollbar.Language
        /// Default: csharp
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// .NET Framework version
        ///
        /// Setting: Rollbar.Framework
        /// Default: ".NET" + <see cref="System.Environment.Version"/>
        /// </summary>
        public string Framework { get; set; }

        /// <summary>
        /// GIT SHA hash of the running code
        ///
        /// Setting: Rollbar.GitSha
        /// Default: None. You have to set this.
        /// </summary>
        public string GitSha { get; set; }

        /// <summary>
        /// Parameters that should be scrubbed (replaced with asterisks) rather than
        /// being sent to rollbar. Such as passwords, secret keys, etc.
        /// </summary>
        public string[] ScrubParams { get; set; }

        /// <summary>
        /// If true, don't attempt to populate the person field in exception reports.
        /// Defaults to false.
        /// </summary>
        public bool ScrubPerson { get; set; }

        /// <summary>
        /// Settings used to serialize the payload to JSON when posting to Rollbar
        /// </summary>
        public JsonSerializerSettings JsonSettings
        {
            get
            {
                return new JsonSerializerSettings
                           {
                               Formatting = Formatting.Indented,
                               NullValueHandling = NullValueHandling.Ignore
                           };
            }
        }

        public Configuration(string accessToken)
        {
            Endpoint = DefaultEndpoint;
            AccessToken = accessToken;
            Environment = DefaultEnvironment;
            Platform = System.Environment.OSVersion.ToString();
            Framework = ".NET " + System.Environment.Version;
            Language = DefaultLanguage;
            ScrubParams = DefaultScrubParams;
            ScrubPerson = false;
        }

        /// <summary>
        /// Creates a <see cref="Configuration"/>, reading values from App/Web.config
        /// Rollbar.AccessToken
        /// Rollbar.Endpoint
        /// Rollbar.Environment
        /// Rollbar.Platform
        /// Rollbar.CodeLanguage
        /// Rollbar.Framework
        /// Rollbar.GitSha
        /// </summary>
        /// <returns></returns>
        public static Configuration CreateFromAppConfig()
        {
            var token = GetSetting("Rollbar.AccessToken");

            if (string.IsNullOrEmpty(token))
                throw new ConfigurationErrorsException("Missing access token at Rollbar.AccessToken");

            var conf = new Configuration(token);

            conf.Endpoint = GetSetting("Rollbar.Endpoint") ?? conf.Endpoint;
            conf.Environment = GetSetting("Rollbar.Environment") ?? conf.Environment;
            conf.Platform = GetSetting("Rollbar.Platform") ?? conf.Platform;
            conf.Language = GetSetting("Rollbar.CodeLanguage") ?? conf.Language;
            conf.Framework = GetSetting("Rolllbar.Framework") ?? conf.Framework;
            conf.GitSha = GetSetting("Rollbar.GitSha");

            var scrubParams = GetSetting("Rollbar.ScrubParams");
            conf.ScrubParams = scrubParams == null ? DefaultScrubParams : scrubParams.Split(',');

            conf.ScrubPerson = GetBoolSetting("Rollbar.ScrubPerson", conf.ScrubPerson);

            return conf;
        }

        protected static string GetSetting(string name, string fallback = null)
        {
            var setting = ConfigurationManager.AppSettings[name];
            return string.IsNullOrEmpty(setting) ? fallback : setting;
        }

        protected static bool GetBoolSetting(string name, bool fallback) {
            var setting = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(setting)) return fallback;
            bool val = fallback;
            boolean.TryParse(setting, out val);
            return val;
        }
    }
}
