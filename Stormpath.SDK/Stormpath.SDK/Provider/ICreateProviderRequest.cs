// <copyright file="ICreateProviderRequest.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Provider
{
    /// <summary>
    /// Placeholder for all the information pertaining to a Provider when attempting to create a
    /// new Provider-based <see cref="Directory.IDirectory">Directory</see> in Stormpath.
    /// </summary>
    /// <seealso cref="ICreateProviderRequestBuilder{T}"/>
    public interface ICreateProviderRequest
    {
        /// <summary>
        /// Returns the Provider instance containing all the Provider information to be used
        /// when creating a Provider-based directory in Stormpath.
        /// </summary>
        /// <returns>The Provider instance containing all the Provider information to be used
        /// when creating a Provider-based directory in Stormpath.</returns>
        IProvider GetProvider();
    }
}
