// <copyright file="DefaultCustomData.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Impl.CustomData
{
    internal sealed class DefaultCustomData : AbstractInstanceResource, ICustomData, ICustomDataSync
    {
        private static readonly List<string> ReservedKeys = new List<string>()
        {
            HrefPropertyName, CreatedAtPropertyName, ModifiedAtPropertyName
        };

        private static readonly List<string> FutureReservedKeys = new List<string>()
        {
            "meta", "spMeta", "spmeta", "ionmeta", "ionMeta"
        };

        // Matches any character in a-z, A-Z, 0-9, _, -  (but cannot start with -)
        private static readonly Regex ValidKeyCharactersRegex = new Regex("^[a-zA-Z0-9_]+[a-zA-Z0-9_-]*$", RegexOptions.Compiled);

        public DefaultCustomData(ResourceData data)
            : base(data)
        {
        }

        private ICustomData AsInterface => this;

        private static bool IsValidKey(string possibleKey)
        {
            if (possibleKey.Length > 255)
                return false;

            bool isValidCharacters = ValidKeyCharactersRegex.IsMatch(possibleKey);
            if (!isValidCharacters)
                return false;

            bool isReservedKeyword = ReservedKeys.Contains(possibleKey) || FutureReservedKeys.Contains(possibleKey);
            if (isReservedKeyword)
                return false;

            return true;
        }

        private List<string> GetAvailableKeys()
        {
            var keys = new List<string>();
            keys.AddRange(this.GetResourceData()?.GetUpdatedPropertyNames());
            keys.AddRange(this.GetResourceData()?.GetPropertyNames());

            var deletedProperties = this.GetResourceData()?.GetDeletedPropertyNames();
            keys.RemoveAll(x => deletedProperties.Contains(x));

            return keys;
        }

        internal bool HasDeletedProperties()
            => this.GetResourceData()?.GetDeletedPropertyNames().Any() ?? false;

        internal bool HasUpdatedProperties()
            => this.GetResourceData()?.GetUpdatedPropertyNames().Any() ?? false;

        object ICustomData.this[string key]
        {
            get { return this.AsInterface.Get(key); }

            set { this.AsInterface.Put(key, value); }
        }

        IReadOnlyCollection<string> ICustomData.Keys
            => this.GetAvailableKeys();

        IReadOnlyCollection<object> ICustomData.Values
        {
            get
            {
                var values = new List<object>();

                this.GetAvailableKeys().ForEach(x =>
                    values.Add(this.AsInterface.Get(x)));

                return values;
            }
        }

        internal IReadOnlyDictionary<string, object> GetUpdatedProperties()
            => this.GetResourceData()?.GetUpdatedProperties();

        void ICustomData.Clear()
        {
            var keysToClear = this
                .GetAvailableKeys()
                .Except(ReservedKeys)
                .ToList();

            keysToClear.ForEach(key => this.AsInterface.Remove(key));
        }

        bool ICustomData.ContainsKey(string key)
            => this.GetAvailableKeys().Contains(key);

        object ICustomData.Get(string key)
            => this.GetProperty(key);

        void ICustomData.Put(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (!IsValidKey(key))
                throw new ArgumentOutOfRangeException($"{key} is not a valid key name.");

            this.SetProperty(key, value);
        }

        void ICustomData.Put(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            bool isEmpty = !keyValuePairs?.Any() ?? true;
            if (isEmpty)
                return;

            foreach (var kvp in keyValuePairs)
            {
                this.AsInterface.Put(kvp.Key, kvp.Value);
            }
        }

        void ICustomData.Put(object customData)
        {
            // This is probably an anonymous type. But, a user could have passed
            // a type meant for a different overload as an object, so
            // we need to do a little investigation to find out
            // (fail fast if it's just a null)
            if (customData == null)
                return;

            var asEnumerable = customData as IEnumerable<KeyValuePair<object, string>>;
            if (asEnumerable != null)
            {
                this.AsInterface.Put(asEnumerable);
                return;
            }

            if (customData is KeyValuePair<string, object>)
            {
                this.AsInterface.Put((KeyValuePair<string, object>)customData);
                return;
            }

            // Assume it's an anonymous type and convert
            var anonymousAsDictionary = AnonymousType.ToDictionary(customData);
            this.AsInterface.Put(anonymousAsDictionary);
        }

        void ICustomData.Put(KeyValuePair<string, object> keyValuePair)
            => this.AsInterface.Put(keyValuePair.Key, keyValuePair.Value);

        object ICustomData.Remove(string key)
        {
            if (ReservedKeys.Contains(key))
                throw new ArgumentOutOfRangeException(nameof(key), $"{key} is a reserved key and cannot be removed.");

            return this.GetResourceData()?.RemoveProperty(key);
        }

        bool ICustomData.TryGetValue(string key, out object value)
        {
            value = this.AsInterface.Get(key);

            return value != null;
        }

        bool ICustomData.IsEmptyOrDefault()
            => this.GetAvailableKeys().All(x => ReservedKeys.Contains(x));

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            foreach (var key in this.GetAvailableKeys())
            {
                yield return new KeyValuePair<string, object>(key, this.AsInterface[key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.AsInterface.GetEnumerator();

        public async Task<bool> DeleteRemovedPropertiesAsync(string parentHref, CancellationToken cancellationToken)
        {
            var propertyDeletionTasks = this.GetResourceData()?.GetDeletedPropertyNames()
                .Select(async x =>
                {
                    var successful =
                        await this.GetInternalAsyncDataStore().DeletePropertyAsync(parentHref, x, cancellationToken).ConfigureAwait(false)
                        && (this.GetResourceData()?.OnDeletingRemovedProperty(x) ?? false);
                    return successful;
                });

            var results = await Task.WhenAll(propertyDeletionTasks).ConfigureAwait(false);

            return results.All(x => x == true);
        }

        public bool DeleteRemovedProperties(string parentHref)
        {
            var results = new List<bool>();
            foreach (var propName in this.GetResourceData()?.GetDeletedPropertyNames())
            {
                var successful = this.GetInternalSyncDataStore().DeleteProperty(parentHref, propName)
                    && (this.GetResourceData()?.OnDeletingRemovedProperty(propName) ?? false);
                results.Add(successful);
            }

            return results.All(x => x == true);
        }

        async Task<ICustomData> ISaveable<ICustomData>.SaveAsync(CancellationToken cancellationToken)
        {
            if (this.HasDeletedProperties())
                await this.DeleteRemovedPropertiesAsync(this.AsInterface.Href, cancellationToken).ConfigureAwait(false);

            return await this.GetInternalAsyncDataStore().SaveAsync<ICustomData>(this, cancellationToken).ConfigureAwait(false);
        }

        ICustomData ISaveableSync<ICustomData>.Save()
        {
            if (this.HasDeletedProperties())
                this.DeleteRemovedProperties(this.AsInterface.Href);

            return this.GetInternalSyncDataStore().Save<ICustomData>(this);
        }

        Task<bool> IDeletable.DeleteAsync(CancellationToken cancellationToken)
            => this.GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        bool IDeletableSync.Delete()
            => this.GetInternalSyncDataStore().Delete(this);
    }
}
