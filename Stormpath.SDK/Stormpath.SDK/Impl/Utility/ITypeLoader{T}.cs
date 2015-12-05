// <copyright file="ITypeLoader{T}.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Utility
{
    /// <summary>
    /// Represents an assembly loader that can instantiate an object from a given assembly.
    /// </summary>
    /// <typeparam name="T">The type to instantiate.</typeparam>
    internal interface ITypeLoader<T>
        where T : class
    {
        /// <summary>
        /// Attempts to instantiate the object.
        /// </summary>
        /// <param name="instance">The new object instance.</param>
        /// <param name="constructorArguments">Arguments to pass to the target constructor.</param>
        /// <returns><see langword="true"/> if the object was instantiated successfully; <see langword="false"/> otherwise.</returns>
        bool TryLoad(out T instance, object[] constructorArguments = null);
    }
}
