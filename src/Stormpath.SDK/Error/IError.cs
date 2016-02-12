// <copyright file="IError.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Error
{
    /// <summary>
    /// Represents a Stormpath error message.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Gets the HTTP status code associated with the error.
        /// </summary>
        /// <value>The HTTP status code associated with this error.</value>
        int HttpStatus { get; }

        /// <summary>
        /// Gets the Stormpath error code associated with the error.
        /// </summary>
        /// <value>The Stormpath error code associated with this error. May be identical to <see cref="HttpStatus"/>.</value>
        int Code { get; }

        /// <summary>
        /// Gets a user-friendly error message.
        /// </summary>
        /// <value>A user-friendly error message string.</value>
        string Message { get; }

        /// <summary>
        /// Gets a detailed developer error message.
        /// </summary>
        /// <value>Contains additional details that may be useful for debugging, if any.</value>
        string DeveloperMessage { get; }

        /// <summary>
        /// Gets additional information related to the error from Stormpath.
        /// </summary>
        /// <value>Additional information related to this error, if any.</value>
        string MoreInfo { get; }
    }
}
