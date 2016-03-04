// <copyright file="IAccountCreationOptions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents options for an <see cref="IAccount">Account</see> creation request.
    /// </summary>
    public interface IAccountCreationOptions : ICreationOptions
    {
        /// <summary>
        /// Gets whether to override the default registration workflow of the Login Source.
        /// </summary>
        /// <value>
        /// If set to <see langword="true"/>, the account registration workflow will be triggered no matter what the Login Source configuration is.
        /// If set to <see langword="false"/>, the account registration workflow will <b>NOT</b> be triggered, no matter what the Login Source configuration is.
        /// If you want to ensure the registration workflow behavior matches the Login Source default, leave this <see langword="null"/>.
        /// </value>
        bool? RegistrationWorkflowEnabled { get; }

        /// <summary>
        /// Gets the password format, used for importing passwords.
        /// </summary>
        /// <remarks>This value should be <see langword="null"/> unless you are importing existing password hashes into Stormpath.</remarks>
        /// <value>The password format.</value>
        PasswordFormat PasswordFormat { get; }
    }
}
