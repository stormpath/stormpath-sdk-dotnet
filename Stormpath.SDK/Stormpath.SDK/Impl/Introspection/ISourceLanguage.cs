// <copyright file="ISourceLanguage.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Impl.Introspection
{
    /// <summary>
    /// Represents the original source language of a compiled assembly.
    /// </summary>
    internal interface ISourceLanguage
    {
        /// <summary>
        /// Gets the name of the source language.
        /// </summary>
        /// <returns>The name of the source language.</returns>
        string ToString();
    }
}