// <copyright file="SamlException.cs" company="Stormpath, Inc.">
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
using System.Runtime.Serialization;
using Stormpath.SDK.Error;
using Stormpath.SDK.Impl.Error;

namespace Stormpath.SDK.Saml
{
    /// <summary>
    /// Represents a SAML-related exception.
    /// </summary>
    [Serializable]
    public class SamlException : ApplicationException, IError, ISerializable
    {
        private readonly string constructedErrorMessage;

        internal readonly DefaultError Error;

        internal SamlException(DefaultError error)
            : base(BuildExceptionMessage(error))
        {
            this.Error = error;
            this.constructedErrorMessage = BuildExceptionMessage(error);
        }

        /// <summary>
        /// Gets the Stormpath error code associated with the error.
        /// </summary>
        /// <value>The Stormpath error code associated with this error. May be identical to <see cref="HttpStatus"/>.</value>
        public int Code => this.Error.Code;

        /// <summary>
        /// Gets a detailed developer error message.
        /// </summary>
        /// <value>Contains additional details that may be useful for debugging, if any.</value>
        public string DeveloperMessage => this.Error.DeveloperMessage;

        /// <summary>
        /// Gets additional information related to the error from Stormpath.
        /// </summary>
        /// <value>Additional information related to this error, if any.</value>
        public string MoreInfo => this.Error.MoreInfo;

        /// <summary>
        /// Gets the HTTP status code associated with the error.
        /// </summary>
        /// <value>The HTTP status code associated with this error.</value>
        public int HttpStatus => this.Error.HttpStatus;

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("code", this.Error.Code);
            info.AddValue("developerMessage", this.Error.DeveloperMessage, typeof(string));
            info.AddValue("moreInfo", this.Error.MoreInfo, typeof(string));
            info.AddValue("status", this.Error.HttpStatus);

            base.GetObjectData(info, context);
        }

        private static string BuildExceptionMessage(IError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return $"HTTP {error.HttpStatus}, Stormpath {error.Code} ({error.MoreInfo}): {error.DeveloperMessage}";
        }
    }
}
