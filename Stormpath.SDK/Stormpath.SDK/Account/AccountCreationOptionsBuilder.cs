// <copyright file="AccountCreationOptionsBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// A builder to construct <see cref="IAccountCreationOptions"/> instances.
    /// </summary>
    public sealed class AccountCreationOptionsBuilder
    {
        /// <summary>
        /// Gets or sets whether to explicitly override the registration workflow of the Login Source for new Accounts.
        /// </summary>
        /// <value>
        /// <para>If set to <c>true</c>, the account registration workflow will be triggered no matter what the Login Source configuration is.</para>
        /// <para>If set to <c>false</c>, the account registration workflow will <b>NOT</b> be triggered, no matter what the Login Source configuration is.</para>
        /// <para>If you want to ensure the registration workflow behavior matches the Login Source default, leave this <c>null</c>.</para>
        /// </value>
        public bool? RegistrationWorkflowEnabled { get; set; }

        /// <summary>
        /// Creates a new <see cref="IAccountCreationOptions"/> instance based on the current builder state.
        /// </summary>
        /// <returns>A new <see cref="IAccountCreationOptions"/> based on the current builder state.</returns>
        public IAccountCreationOptions Build()
        {
            return new DefaultAccountCreationOptions(this.RegistrationWorkflowEnabled);
        }
    }
}
