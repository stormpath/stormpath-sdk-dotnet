// <copyright file="IPasswordResetTokenSync.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// A token returned as part of the reset password workflow that can be accessed synchronously.
    /// </summary>
    internal interface IPasswordResetTokenSync
    {
        /// <summary>
        /// Gets the <see cref="IAccount"/> associated with this password reset token.
        /// </summary>
        /// <returns>The <see cref="IAccount"/> in the reset password workflow.</returns>
        IAccount GetAccount();
    }
}
