// <copyright file="DefaultClientApiKeyBuilder.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

namespace Stormpath.SDK.Impl.Api
{
    using System;
    using Extensions;
    using SDK.Api;
    using Utility;

    internal sealed class DefaultClientApiKeyBuilder : IClientApiKeyBuilder
    {
        private static readonly string DefaultIdPropertyName = "apiKey.id";
        private static readonly string DefaultSecretPropertyName = "apiKey.secret";

        private static readonly string DefaultApiKeyPropertiesFileLocation =
            System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"), ".stormpath\\", "apiKey.properties");

        // Instance fields
        private string _apiKeyId;
        private string _apiKeySecret;
        private System.IO.Stream _apiKeyFileInputStream;
        private string _apiKeyFilePath;
        private string _apiKeyIdPropertyName = DefaultIdPropertyName;
        private string _apiKeySecretPropertyName = DefaultSecretPropertyName;

        // Wrappers for static .NET Framework calls (for easier unit testing)
        private readonly IConfigurationManager _configuration;
        private readonly IEnvironment _environment;
        private readonly IFile _file;

        public DefaultClientApiKeyBuilder(IConfigurationManager configuration, IEnvironment environment, IFile file)
        {
            _configuration = configuration;
            _environment = environment;
            _file = file;
        }

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

        IClientApiKeyBuilder IClientApiKeyBuilder.SetInputStream(System.IO.Stream stream)
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
            var envFileLocation = _environment.GetEnvironmentVariable("STORMPATH_API_KEY_FILE");
            if (!string.IsNullOrEmpty(envFileLocation))
            {
                var envProperties = GetPropertiesFromEnvironmentVariableFileLocation(envFileLocation);
                id = envProperties?.GetProperty(_apiKeyIdPropertyName, defaultValue: id);
                secret = envProperties?.GetProperty(_apiKeySecretPropertyName, defaultValue: secret);
            }

            // 3. Try environment variables directly
            var idFromEnvironment = _environment.GetEnvironmentVariable("STORMPATH_API_KEY_ID");
            var secretFromEnvironment = _environment.GetEnvironmentVariable("STORMPATH_API_KEY_SECRET");
            bool retrievedValuesFromEnvironment = !string.IsNullOrEmpty(idFromEnvironment) && !string.IsNullOrEmpty(secretFromEnvironment);
            if (retrievedValuesFromEnvironment)
            {
                id = idFromEnvironment;
                secret = secretFromEnvironment;
            }

            // 4. Try file location specified by web.config/app.config
            var appConfigFileLocation = _configuration.AppSettings?["STORMPATH_API_KEY_FILE"];
            if (!string.IsNullOrEmpty(appConfigFileLocation))
            {
                var appConfigProperties = GetPropertiesFromAppConfigFileLocation(appConfigFileLocation);
                id = appConfigProperties?.GetProperty(_apiKeyIdPropertyName, defaultValue: id);
                secret = appConfigProperties?.GetProperty(_apiKeySecretPropertyName, defaultValue: secret);
            }

            // 5. Try web.config/app.config keys directly
            var idFromAppConfig = _configuration.AppSettings?["STORMPATH_API_KEY_ID"];
            var secretFromAppConfig = _configuration.AppSettings?["STORMPATH_API_KEY_SECRET"];
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
                var source = _file.ReadAllText(DefaultApiKeyPropertiesFileLocation);
                return new Properties(source);
            }
            catch
            {
                var msg =
                    $"Unable to find or load default API Key properties file [{DefaultApiKeyPropertiesFileLocation}] " +
                    "This can safely be ignored as this is a fallback location - other more specific locations will be checked.";

                // todo - log (catch exception for this)
                return null;
            }
        }

        private Properties GetPropertiesFromEnvironmentVariableFileLocation(string path)
        {
            try
            {
                var source = _file.ReadAllText(path);
                var properties = new Properties(source);
                return properties;
            }
            catch
            {
                var msg =
                    $"Unable to load API Key properties file [{path}] specified by environment variable " +
                    "STORMPATH_API_KEY_FILE. This can safely be ignored as this is a fallback location - " +
                    "other more specific locations will be checked.";

                // todo - log (catch exception for this)
                return null;
            }
        }

        private Properties GetPropertiesFromAppConfigFileLocation(string path)
        {
            try
            {
                var source = _file.ReadAllText(path);
                var properties = new Properties(source);
                return properties;
            }
            catch
            {
                var msg =
                    $"Unable to load API Key properties file [{path}] specified by config key " +
                    "STORMPATH_API_KEY_FILE. This can safely be ignored as this is a fallback location - " +
                    "other more specific locations will be checked.";

                // todo - log (catch exception for this)
                return null;
            }
        }

        private Properties GetPropertiesFromFile()
        {
            try
            {
                var source = _file.ReadAllText(_apiKeyFilePath);
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
            if (!_apiKeyFileInputStream.CanRead)
            {
                return null;
            }

            try
            {
                using (var reader = new System.IO.StreamReader(_apiKeyFileInputStream))
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
