// <copyright file="IIdSiteResultAsyncListener.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;

namespace Stormpath.SDK.IdSite
{
    /// <summary>
    /// Listener interface to get asynchronous notifications about effective operations of the ID Site invocation
    /// (registration, authentication, or logout).
    /// </summary>
    public interface IIdSiteResultAsyncListener
    {
        /// <summary>
        /// This method will be invoked if a successful registration operation takes place on ID Site.
        /// </summary>
        /// <param name="result">The </param>
        /// <returns>A Task that indicates the completion of the method.</returns>
        Task OnRegisteredAsync(IRegistrationResult result);

        /// <summary>
        /// This method will be invoked if a successful registration operation takes place on ID Site.
        /// </summary>
        /// <param name="result">The </param>
        /// <returns>A Task that indicates the completion of the method.</returns>
        Task OnAuthenticatedAsync(IAuthenticationResult result);

        /// <summary>
        /// This method will be invoked if a successful registration operation takes place on ID Site.
        /// </summary>
        /// <param name="result">The </param>
        /// <returns>A Task that indicates the completion of the method.</returns>
        Task OnLogoutAsync(ILogoutResult result);
    }
}
