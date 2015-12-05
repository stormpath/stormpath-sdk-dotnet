// <copyright file="DefaultError.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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

using System;
using System.Collections.Generic;
using Stormpath.SDK.Error;
using Stormpath.SDK.Http;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Error
{
    [Serializable]
    internal sealed class DefaultError : IError
    {
        private static readonly string StatusPropertyName = "status";
        private static readonly string CodePropertyName = "code";
        private static readonly string MessagePropertyName = "message";
        private static readonly string DevMessagePropertyName = "developerMessage";
        private static readonly string MoreInfoPropertyName = "moreInfo";

        private readonly IReadOnlyDictionary<string, object> properties;

        public DefaultError(Map properties)
        {
            this.properties = new Dictionary<string, object>(properties);
        }

        private T GetProperty<T>(string propertyName)
        {
            object value = null;
            if (!this.properties.TryGetValue(propertyName, out value))
            {
                return default(T);
            }

            return (T)value;
        }

        public int Code => this.GetProperty<int>(CodePropertyName);

        public string DeveloperMessage => this.GetProperty<string>(DevMessagePropertyName);

        public string Message => this.GetProperty<string>(MessagePropertyName);

        public string MoreInfo => this.GetProperty<string>(MoreInfoPropertyName);

        public int HttpStatus => this.GetProperty<int>(StatusPropertyName);

        public static DefaultError FromHttpResponse(IHttpResponse response)
        {
            return new DefaultError(new Dictionary<string, object>()
            {
                { "status", response.StatusCode },
                { "code", response.StatusCode },
                { "moreInfo", "HTTP error" },
                { "developerMessage", response.ResponsePhrase }
            });
        }

        public static DefaultError WithMessage(string developerMessage)
        {
            return new DefaultError(new Dictionary<string, object>()
            {
                { "status", 0 },
                { "code", 0 },
                { "moreInfo", "HTTP error" },
                { "developerMessage", developerMessage }
            });
        }
    }
}
