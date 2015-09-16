// <copyright file="IAccountCreationOptions.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents options for an <see cref="IAccount"/> creation request.
    /// </summary>
    public interface IAccountCreationOptions : ICreationOptions
    {
        /// <summary>
        /// Represents an optional override of the registration workflow of the Login Source for new Accounts.
        /// <para>If set to <c>true</c>, the account registration workflow will be triggered no matter what the Login Source configuration is.</para>
        /// <para>If set to <c>false</c>, the account registration workflow will <b>NOT</b> be triggered, no matter what the Login Source configuration is.</para>
        /// <para>If <c>null</c>, the registration workflow behavior matches the Login Source default.</para>
        /// </summary>
        bool? RegistrationWorkflowEnabled { get; }
    }
}
