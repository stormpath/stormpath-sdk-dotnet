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
        private string apiKeyId;
        private string apiKeySecret;
        private System.IO.Stream apiKeyFileInputStream;
        private string apiKeyFilePath;
        private string apiKeyIdPropertyName = DefaultIdPropertyName;
        private string apiKeySecretPropertyName = DefaultSecretPropertyName;

        // Wrappers for static .NET Framework calls (for easier unit testing)
        private readonly IConfigurationManager config;
        private readonly IEnvironment env;
        private readonly IFile file;

        public DefaultClientApiKeyBuilder(IConfigurationManager configuration, IEnvironment environment, IFile file)
        {
            this.config = configuration;
            this.env = environment;
            this.file = file;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetId(string id)
        {
            this.apiKeyId = id;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetSecret(string secret)
        {
            this.apiKeySecret = secret;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetInputStream(System.IO.Stream stream)
        {
            this.apiKeyFileInputStream = stream;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetFileLocation(string path)
        {
            this.apiKeyFilePath = path;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetIdPropertyName(string idPropertyName)
        {
            this.apiKeyIdPropertyName = idPropertyName;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetSecretPropertyName(string secretPropertyName)
        {
            this.apiKeySecretPropertyName = secretPropertyName;
            return this;
        }

        IClientApiKey IClientApiKeyBuilder.Build()
        {
            // 1. Try to load default API key properties file. Lowest priority
            var defaultProperties = GetDefaultApiKeyFileProperties();
            string id = defaultProperties?.GetProperty(this.apiKeyIdPropertyName);
            string secret = defaultProperties?.GetProperty(this.apiKeySecretPropertyName);

            // 2. Try file location specified by environment variables
            var envFileLocation = this.env.GetEnvironmentVariable("STORMPATH_API_KEY_FILE");
            if (!string.IsNullOrEmpty(envFileLocation))
            {
                var envProperties = GetPropertiesFromEnvironmentVariableFileLocation(envFileLocation);
                id = envProperties?.GetProperty(this.apiKeyIdPropertyName, defaultValue: id);
                secret = envProperties?.GetProperty(this.apiKeySecretPropertyName, defaultValue: secret);
            }

            // 3. Try environment variables directly
            var idFromEnvironment = this.env.GetEnvironmentVariable("STORMPATH_API_KEY_ID");
            var secretFromEnvironment = this.env.GetEnvironmentVariable("STORMPATH_API_KEY_SECRET");
            bool didRetrieveValuesFromEnvironment = !string.IsNullOrEmpty(idFromEnvironment) && !string.IsNullOrEmpty(secretFromEnvironment);
            if (didRetrieveValuesFromEnvironment)
            {
                id = idFromEnvironment;
                secret = secretFromEnvironment;
            }

            // 4. Try file location specified by web.config/app.config
            var appConfigFileLocation = this.config.AppSettings?["STORMPATH_API_KEY_FILE"];
            if (!string.IsNullOrEmpty(appConfigFileLocation))
            {
                var appConfigProperties = GetPropertiesFromAppConfigFileLocation(appConfigFileLocation);
                id = appConfigProperties?.GetProperty(this.apiKeyIdPropertyName, defaultValue: id);
                secret = appConfigProperties?.GetProperty(this.apiKeySecretPropertyName, defaultValue: secret);
            }

            // 5. Try web.config/app.config keys directly
            var idFromAppConfig = this.config.AppSettings?["STORMPATH_API_KEY_ID"];
            var secretFromAppConfig = this.config.AppSettings?["STORMPATH_API_KEY_SECRET"];
            bool didRetrieveValuesFromAppConfig = !string.IsNullOrEmpty(idFromAppConfig) && !string.IsNullOrEmpty(secretFromAppConfig);
            if (didRetrieveValuesFromAppConfig)
            {
                id = idFromAppConfig;
                secret = secretFromAppConfig;
            }

            // 6. Try configured property file
            if (!string.IsNullOrEmpty(this.apiKeyFilePath))
            {
                var fileProperties = GetPropertiesFromFile();
                id = fileProperties?.GetProperty(this.apiKeyIdPropertyName, defaultValue: id);
                secret = fileProperties?.GetProperty(this.apiKeySecretPropertyName, defaultValue: secret);
            }

            // 7. Try an input stream that was passed to us
            if (this.apiKeyFileInputStream != null)
            {
                var streamProperties = GetPropertiesFromStream();
                id = streamProperties?.GetProperty(this.apiKeyIdPropertyName, defaultValue: id);
                secret = streamProperties?.GetProperty(this.apiKeySecretPropertyName, defaultValue: secret);
            }

            // 8. Explicitly-configured values always take precedence
            id = this.apiKeyId.OrIfEmptyUse(id);
            secret = this.apiKeySecret.OrIfEmptyUse(secret);

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
                var source = this.file.ReadAllText(DefaultApiKeyPropertiesFileLocation);
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
                var source = this.file.ReadAllText(path);
                var properties = new Properties(source);
                return properties;
            }
            catch
            {
                var msg =
                    $"Unable to load API Key properties file [{path}] specified by environment variable " +
                    "STORMPATH_API_KEYthis.File. This can safely be ignored as this is a fallback location - " +
                    "other more specific locations will be checked.";

                // todo - log (catch exception for this)
                return null;
            }
        }

        private Properties GetPropertiesFromAppConfigFileLocation(string path)
        {
            try
            {
                var source = this.file.ReadAllText(path);
                var properties = new Properties(source);
                return properties;
            }
            catch
            {
                var msg =
                    $"Unable to load API Key properties file [{path}] specified by config key " +
                    "STORMPATH_API_KEYthis.File. This can safely be ignored as this is a fallback location - " +
                    "other more specific locations will be checked.";

                // todo - log (catch exception for this)
                return null;
            }
        }

        private Properties GetPropertiesFromFile()
        {
            try
            {
                var source = this.file.ReadAllText(this.apiKeyFilePath);
                var properties = new Properties(source);
                return properties;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Unable to read properties from specified file location [{this.apiKeyFilePath}]", ex);
            }
        }

        private Properties GetPropertiesFromStream()
        {
            if (!this.apiKeyFileInputStream.CanRead) return null;

            try
            {
                using (var reader = new System.IO.StreamReader(this.apiKeyFileInputStream))
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
