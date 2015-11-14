// <copyright file="ILoggerConsumer.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Logging
{
    /// <summary>
    /// Represents a class that depends on <see cref="ILogger"/>.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    public interface ILoggerConsumer<out T>
    {
        /// <summary>
        /// Sets an optional logger to send trace and debug messages to.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The source object for method chaining.</returns>
        T SetLogger(ILogger logger);
    }
}
