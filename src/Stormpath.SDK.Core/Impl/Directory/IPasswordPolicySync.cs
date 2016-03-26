// <copyright file="IPasswordPolicySync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Directory;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Directory
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on an <see cref="IPasswordPolicy">Password Policy</see>.
    /// </summary>
    internal interface IPasswordPolicySync : ISaveableSync<IPasswordPolicy>
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IPasswordPolicy.GetPasswordStrengthPolicyAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The Password Strength Policy for this Directory.</returns>
        IPasswordStrengthPolicy GetPasswordStrengthPolicy();
    }
}
