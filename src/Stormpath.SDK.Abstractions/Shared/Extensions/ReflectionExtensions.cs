// <copyright file="ReflectionExtensions.cs" company="Stormpath, Inc.">
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

using System;
using System.Reflection;
using System.Linq;

namespace Stormpath.SDK.Shared.Extensions
{
    /// <summary>
    /// Extension methods for working with the Reflection API.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the default (parameterless) constructor for a given <paramref name="typeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <returns>The constructor if found; otherwise, <see langword="null"/>.</returns>
        public static ConstructorInfo GetDefaultConstructor(this TypeInfo typeInfo)
            => typeInfo.GetConstructor(new Type[] { });

        /// <summary>
        /// Gets the constructor that accepts the given <paramref name="parameterTypes"/>.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>The constructor if found; otherwise, <see langword="null"/>.</returns>
        public static ConstructorInfo GetConstructor(this TypeInfo typeInfo, Type[] parameterTypes)
        {
            var constructor = typeInfo.DeclaredConstructors
                .Where(c => 
                    c.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes)
                    && !c.IsStatic)
                .SingleOrDefault();

            return constructor;
        }
    }
}
