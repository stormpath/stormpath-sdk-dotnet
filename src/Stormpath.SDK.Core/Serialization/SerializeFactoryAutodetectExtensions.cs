// <copyright file="SerializeFactoryAutodetectExtensions.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Serialization;

namespace Stormpath.SDK.Serialization
{
    /// <summary>
    /// Provides access to the plugin autodetection behavior.
    /// </summary>
    public static class SerializeFactoryAutodetectExtensions
    {
        /// <summary>
        /// Automatically detects the serializer plugin to use.
        /// </summary>
        /// <param name="factory">The plugin factory.</param>
        /// <returns>A builder capable of building <see cref="IJsonSerializer"/> instances.</returns>
        [Obsolete("Will be removed in 1.0. Instead, manually specify a serializer, e.g. Serializers.Create().JsonNetSerializer()")]
        public static ISerializerBuilder AutoDetect (this ISerializerFactory factory)
        {
#if NET45
            Type defaultSerializerType = null;
            try
            {
                defaultSerializerType = DefaultSerializerLoader.Load();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while loading the default serializer. See the inner exception for details.", ex);
            }

            return new AbstractSerializerBuilder<IJsonSerializer>(defaultSerializerType);
#else
            throw new Exception("Plugin autodetection is not supported on this platform. Specify the plugin manually.");
#endif
        }
    }
}
