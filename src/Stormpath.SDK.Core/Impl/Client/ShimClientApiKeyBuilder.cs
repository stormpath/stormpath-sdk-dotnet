// <copyright file="ShimClientApiKeyBuilder.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.IO;
using Stormpath.Configuration.Abstractions.Immutable;
using Stormpath.SDK.Api;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class ShimClientApiKeyBuilder : IClientApiKeyBuilder
    {
        private readonly ShimAdditionalClientApiKeySettings additionalSettings = new ShimAdditionalClientApiKeySettings();

        private string file;
        private string id;
        private string secret;

        public ShimClientApiKeyBuilder(ILogger logger)
        {
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetFileLocation(string path)
        {
            this.file = path;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetId(string id)
        {
            this.id = id;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetIdPropertyName(string idPropertyName)
        {
            this.additionalSettings.IdPropertyName = idPropertyName;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetInputStream(Stream stream)
        {
            this.additionalSettings.InputStream = stream;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetSecret(string secret)
        {
            this.secret = secret;
            return this;
        }

        IClientApiKeyBuilder IClientApiKeyBuilder.SetSecretPropertyName(string secretPropertyName)
        {
            this.additionalSettings.SecretPropertyName = secretPropertyName;
            return this;
        }

        IClientApiKey IClientApiKeyBuilder.Build()
        {
            return new ShimClientApiKey(
                new ClientApiKeyConfiguration(this.file, this.id, this.secret),
                additionalSettings);
        }
    }
}
