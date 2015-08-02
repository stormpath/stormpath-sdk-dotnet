using Stormpath.SDK.Api;
using Stormpath.SDK.Impl.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Api
{
    internal class ClientApiKey : IClientApiKey, IEquatable<ClientApiKey>
    {
        private readonly string _id;
        private readonly string _secret;

        private static readonly GenericComparer<ClientApiKey> _comparer = new GenericComparer<ClientApiKey>(
            (ClientApiKey x, ClientApiKey y) =>
            {
                return x._id == y._id && x._secret == y._secret;
            },
            (ClientApiKey x) =>
            {
                return HashCode.Start
                    .Hash(x._id);
            });


        internal ClientApiKey(string id, string secret)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("API key ID cannot be null or empty.");

            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("API key secret cannot be null or empty.");

            _id = id;
            _secret = secret;
        }

        string IClientApiKey.GetId()
        {
            return _id;
        }

        string IClientApiKey.GetSecret()
        {
            return _secret;
        }

        #region Boilerplate comparison and equality stuff

        public bool Equals(ClientApiKey other)
        {
            return Equals((object)other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ClientApiKey;
            if (obj == null)
                return false;

            return _comparer.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return _comparer.GetHashCode(this);
        }

        public static bool operator ==(ClientApiKey x, ClientApiKey y)
        {
            return _comparer.Equals(x, y);
        }

        public static bool operator !=(ClientApiKey x, ClientApiKey y)
        {
            return !(x == y);
        }

        #endregion
    }
}
