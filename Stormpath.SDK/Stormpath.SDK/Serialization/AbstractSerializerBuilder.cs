// <copyright file="AbstractSerializerBuilder.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Serialization
{
    /// <summary>
    /// Builder class capable of constructing <see cref="IJsonSerializer"/> instances using the specified properties.
    /// </summary>
    /// <remarks>
    /// The constructed type <b>must</b> have a public or private parameterless constructor.
    /// </remarks>
    /// <typeparam name="T">The concrete type being constructed.</typeparam>
    public sealed class AbstractSerializerBuilder<T> : ISerializerBuilder
        where T : IJsonSerializer
    {
        private readonly ConstructorInfo targetConstructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSerializerBuilder{T}"/> class.
        /// </summary>
        public AbstractSerializerBuilder()
            : this(typeof(T))
        {
        }

        internal AbstractSerializerBuilder(Type targetType)
        {
            this.targetConstructor = targetType.FindConstructor(
                new Type[] { },
                findPrivate: true);

            if (this.targetConstructor == null)
            {
                throw new NotSupportedException(
                    $"The serializer '{typeof(T)?.Name} does not have a supported constructor. Instantiate this serializer directly instead of using ISerializerBuilder.");
            }
        }

        /// <inheritdoc/>
        IJsonSerializer ISerializerBuilder.Build()
        {
            IJsonSerializer instance = null;

            try
            {
                instance = (IJsonSerializer)this.targetConstructor.Invoke(new object[] { });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Unable to build the serializer {typeof(T).Name}; see the inner exception for details. Try instantiating the serializer directly instead of using ISerializerBuilder.",
                    ex);
            }

            if (instance == null)
            {
                throw new Exception($"Unable to build the serializer {typeof(T).Name}. No exception was thrown, but the result was null. Try instantiating the serializer directly instead of using ISerializerBuilder.");
            }

            return instance;
        }
    }
}
