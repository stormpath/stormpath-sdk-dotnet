// <copyright file="VerificationEmailRequestBuilder.cs" company="Stormpath, Inc.">
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

using System.Collections.Generic;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.DataStore;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// A builder to construct <see cref="IVerificationEmailRequest"/> instances.
    /// </summary>
    public sealed class VerificationEmailRequestBuilder
    {
        /// <summary>
        /// Gets or sets the account's login information. Either the username or email identifying the desired account can be used.
        /// </summary>
        /// <value>The username or email identifying the account that will receive the verification email.</value>
        public string Login { get; set; }

        // TODO AccountStore

        /// <summary>
        /// Gets or sets the <see cref="IInternalDataStore"/> used to construct this request.
        /// </summary>
        /// <value>The internal data store used by this client.</value>
        internal IInternalDataStore InternalDataStore { get; set; }

        /// <summary>
        /// Creates a new <see cref="IVerificationEmailRequest"/> instance based on the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IVerificationEmailRequest"/> based on the current builder state.</returns>
        public IVerificationEmailRequest Build()
        {
            var properties = new Dictionary<string, object>() { { "login", this.Login } };
            return new DefaultVerificationEmailRequest(this.InternalDataStore, properties);
        }
    }
}
