// <copyright file="ISerializerFactory.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Serialization
{
    /// <summary>
    /// Represents a factory that can create <see cref="ISerializerBuilder">serializer builders</see>.
    /// </summary>
    public interface ISerializerFactory
    {
        /// <summary>
        /// Use the default serializer.
        /// </summary>
        /// <remarks>
        /// Dynamically loads the default serializer (currently <c>JsonNetSerializer</c>) by searching the application path.
        /// This method is implicitly called by <see cref="Client.IClientBuilder"/> unless a different <see cref="ISerializerBuilder">serializer builder</see> is specified.
        /// </remarks>
        /// <seealso cref="Client.IClientBuilder.SetSerializer(ISerializerBuilder)"/>
        /// <returns>A <see cref="ISerializerBuilder">builder</see> capable of constructing the default serializer.</returns>
        [Obsolete("Will be removed in 1.0. Instead, manually specify a serializer, e.g. Serializers.Create().JsonNetSerializer()")]
        ISerializerBuilder AutoDetect();
    }
}
