// <copyright file="DefaultError.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using Stormpath.SDK.Error;
using Stormpath.SDK.Http;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.Error
{
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

        // TODO dry this up with ResourceData
        private string GetStringProperty(string name)
            => this.GetProperty(name)?.ToString();

        private int GetIntProperty(string name)
             => Convert.ToInt32(this.GetProperty(name) ?? default(int));

        private object GetProperty(string propertyName)
        {
            object value = null;
            this.properties.TryGetValue(propertyName, out value);
            return value;
        }

        public int Code => this.GetIntProperty(CodePropertyName);

        public string DeveloperMessage => this.GetStringProperty(DevMessagePropertyName);

        public string Message => this.GetStringProperty(MessagePropertyName);

        public string MoreInfo => this.GetStringProperty(MoreInfoPropertyName);

        public int HttpStatus => this.GetIntProperty(StatusPropertyName);

        public static DefaultError FromHttpResponse(IHttpResponse response)
        {
            return new DefaultError(new Dictionary<string, object>()
            {
                { "status", response.StatusCode },
                { "code", response.StatusCode },
                { "moreInfo", "HTTP error" },
                { "developerMessage", response.ResponsePhrase },
                { "message", "An error occurred." },
            });
        }

        public static DefaultError WithMessage(string developerMessage)
        {
            return new DefaultError(new Dictionary<string, object>()
            {
                { "status", 0 },
                { "code", 0 },
                { "moreInfo", "HTTP error" },
                { "developerMessage", developerMessage },
                { "message", "An error occurred." },
            });
        }

        public static DefaultError With(int status, int code, string moreInfo, string developerMessage, string message)
        {
            return new DefaultError(new Dictionary<string, object>()
            {
                { "status", status },
                { "code", code },
                { "moreInfo", moreInfo },
                { "developerMessage", developerMessage },
                { "message", message },
            });
        }
    }
}
