using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Stormpath.SDK.Impl.Utility;

namespace Stormpath.SDK.Impl.Api
{
    internal sealed class DefaultClientApiKeyBuilder : IClientApiKeyBuilder
    {
        private static readonly string DefaultIdPropertyName = "apiKey.id";
        private static readonly string DefaultSecretPropertyName = "apiKey.secret";

        private static readonly string DefaultApiKeyPropertiesFileLocation =
            Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), ".stormpath\\", "apiKey.properties");

        private string _apiKeyId;
        private string _apiKeySecret;
        private Stream _apiKeyFileInputStream;
        private string _apiKeyFilePath;
        private string _apiKeyIdPropertyName = DefaultIdPropertyName;
        private string _apiKeySecretPropertyName = DefaultSecretPropertyName;

        IClientApiKeyBuilder IClientApiKeyBuilder.SetId(string id)
        {
            _apiKeyId = id;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetSecret(string secret)
        {
            _apiKeySecret = secret;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetInputStream(Stream stream)
        {
            _apiKeyFileInputStream = stream;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetFileLocation(string path)
        {
            _apiKeyFilePath = path;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetIdPropertyName(string idPropertyName)
        {
            _apiKeyIdPropertyName = idPropertyName;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetSecretPropertyName(string secretPropertyName)
        {
            _apiKeySecretPropertyName = secretPropertyName;
            return this;
        }


        IClientApiKey IClientApiKeyBuilder.Build()
        {
            // 1. Try to load default API key properties file. Lowest priority
            var defaultProperties = GetDefaultApiKeyFileProperties();
            string id = defaultProperties?.GetProperty(_apiKeyIdPropertyName);
            string secret = defaultProperties?.GetProperty(_apiKeySecretPropertyName);

            // 2. Try file location specified by environment variables
            var envFileLocation = Environment.GetEnvironmentVariable("STORMPATH_API_KEY_FILE");
            if (!string.IsNullOrEmpty(envFileLocation))
            {
                var envProperties = GetPropertiesFromEnvironmentVariableFileLocation(envFileLocation);
                id = envProperties?.GetProperty(_apiKeyIdPropertyName, defaultValue: id);
                secret = envProperties?.GetProperty(_apiKeySecretPropertyName, defaultValue: secret);
            }

            // 3. Try environment variables directly
            var idFromEnvironment = Environment.GetEnvironmentVariable("STORMPATH_API_KEY_ID");
            var secretFromEnvironment = Environment.GetEnvironmentVariable("STORMPATH_API_KEY_SECRET");
            bool retrievedValuesFromEnvironment = !string.IsNullOrEmpty(idFromEnvironment) && !string.IsNullOrEmpty(secretFromEnvironment);
            if (retrievedValuesFromEnvironment)
            {
                id = idFromEnvironment;
                secret = secretFromEnvironment;
            }

            // 4. Try file location specified by web.config/app.config
            var appConfigFileLocation = ConfigurationManager.AppSettings["STORMPATH_API_KEY_FILE"];
            if (!string.IsNullOrEmpty(appConfigFileLocation))
            {
                var appConfigProperties = GetPropertiesFromAppConfigFileLocation(appConfigFileLocation);
                id = appConfigProperties?.GetProperty(_apiKeyIdPropertyName, defaultValue: id);
                secret = appConfigProperties?.GetProperty(_apiKeySecretPropertyName, defaultValue: secret);
            }

            // 5. Try web.config/app.config keys directly
            var idFromAppConfig = ConfigurationManager.AppSettings["STORMPATH_API_KEY_ID"];
            var secretFromAppConfig = ConfigurationManager.AppSettings["STORMPATH_API_KEY_SECRET"];
            bool retrievedValuesFromAppConfig = !string.IsNullOrEmpty(idFromAppConfig) && !string.IsNullOrEmpty(secretFromAppConfig);
            if (retrievedValuesFromAppConfig)
            {
                id = idFromAppConfig;
                secret = secretFromAppConfig;
            }

            // 6. Try configured property file
            if (!string.IsNullOrEmpty(_apiKeyFilePath))
            {
                var fileProperties = GetPropertiesFromFile();
                id = fileProperties?.GetProperty(_apiKeyIdPropertyName, defaultValue: id);
                secret = fileProperties?.GetProperty(_apiKeySecretPropertyName, defaultValue: secret);
            }

            // 7. Try an input stream that was passed to us
            if (_apiKeyFileInputStream != null)
            {
                var streamProperties = GetPropertiesFromStream();
                id = streamProperties?.GetProperty(_apiKeyIdPropertyName, defaultValue: id);
                secret = streamProperties?.GetProperty(_apiKeySecretPropertyName, defaultValue: secret);
            }

            // 8. Explicitly-configured values always take precedence
            id = _apiKeyId.OrIfEmptyUse(id);
            secret = _apiKeySecret.OrIfEmptyUse(secret);

            if (string.IsNullOrEmpty(id))
            {
                var message = "Unable to find an API Key ID, either from explicit configuration (for example, " +
                    nameof(IClientApiKeyBuilder) + ".setApiKeyId), or from a file location.\r\n" +
                    "Please provide the API Key ID by one of these methods.";
                throw new ApplicationException(message);
            }

            if (string.IsNullOrEmpty(secret))
            {
                var message = "Unable to find an API Key Secret, either from explicit configuration (for example, " +
                    nameof(IClientApiKeyBuilder) + ".setApiKeySecret), or from a file location.\r\n" +
                    "Please provide the API Key Secret by one of these methods.";
                throw new ApplicationException(message);
            }

            return new ClientApiKey(id, secret);
        }

        private Properties GetDefaultApiKeyFileProperties()
        {
            try
            {
                var source = File.ReadAllText(DefaultApiKeyPropertiesFileLocation);
                return new Properties(source);
            }
            catch //(Exception ignored)
            {
                var msg =
                    $"Unable to find or load default API Key properties file [{DefaultApiKeyPropertiesFileLocation}] " +
                    "This can safely be ignored as this is a fallback location - other more specific locations will be checked.";
                // todo - log
                return null;
            }
        }

        private Properties GetPropertiesFromEnvironmentVariableFileLocation(string path)
        {
            try
            {
                var source = File.ReadAllText(path);
                var properties = new Properties(source);
                return properties;
            }
            catch //(Exception ignored)
            {
                var msg =
                    $"Unable to load API Key properties file [{path}] specified by environment variable " +
                    "STORMPATH_API_KEY_FILE. This can safely be ignored as this is a fallback location - " +
                    "other more specific locations will be checked.";
                // todo - log
                return null;
            }
        }

        private Properties GetPropertiesFromAppConfigFileLocation(string path)
        {
            try
            {
                var source = File.ReadAllText(path);
                var properties = new Properties(source);
                return properties;
            }
            catch //(Exception ignored)
            {
                var msg =
                    $"Unable to load API Key properties file [{path}] specified by config key " +
                    "STORMPATH_API_KEY_FILE. This can safely be ignored as this is a fallback location - " +
                    "other more specific locations will be checked.";
                // todo - log
                return null;
            }
        }

        private Properties GetPropertiesFromFile()
        {
            try
            {
                var source = File.ReadAllText(_apiKeyFilePath);
                var properties = new Properties(source);
                return properties;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Unable to read properties from specified file location [{_apiKeyFilePath}]", ex);
            }
        }

        private Properties GetPropertiesFromStream()
        {
            if (!_apiKeyFileInputStream.CanRead) return null;
            try
            {
                using (var reader = new StreamReader(_apiKeyFileInputStream))
                {
                    var source = reader.ReadToEnd();
                    return new Properties(source);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to read properties from specified input stream.", ex);
            }
        }

    }
}
