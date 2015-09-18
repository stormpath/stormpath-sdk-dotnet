// <copyright file="DefaultAccountStore.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.AccountStore
{
    internal sealed class DefaultAccountStore : AbstractInstanceResource, IAccountStore
    {
        public DefaultAccountStore(IInternalDataStore dataStore)
            : base(dataStore)
        {
        }

        public DefaultAccountStore(IInternalDataStore dataStore, IDictionary<string, object> properties)
            : base(dataStore, properties)
        {
        }
    }
}
