// <copyright file="IResourceConverter.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Impl.Resource;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore
{
    /// <summary>
    /// Converts resources into <see cref="Map"/> instances.
    /// </summary>
    internal interface IResourceConverter
    {
        /// <summary>
        /// Converts an <see cref="AbstractResource"/> instance into a <see cref="Map"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>A <see cref="Map"/> of key-value pairs.</returns>
        Map ToMap(AbstractResource resource);
    }
}
