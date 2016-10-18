using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultFactorCollection : CollectionResourceQueryable<IFactor>, IFactorCollection
    {
        public DefaultFactorCollection(string collectionHref, IInternalDataStore dataStore)
            : base(collectionHref, dataStore)
        {
        }

        public Task<ISmsFactor> AddAsync(
            SmsFactorCreationOptions options,
            CancellationToken cancellationToken)
        {
            var smsFactorCreationData = new SmsFactorCreationData
            {
                Phone = new SmsFactorCreationPhoneData
                {
                    Number = options.Number
                },
                Status = options.Status
            };

            var parameters = new Dictionary<string, string>();
            if (options.Challenge)
            {
                parameters.Add("challenge", "true");
            }

            var creationOptions = new DefaultCreationOptions(parameters);

            return (_dataStore as IInternalAsyncDataStore).CreateAsync<SmsFactorCreationData, ISmsFactor>(
                _collectionHref,
                smsFactorCreationData,
                creationOptions,
                null,
                cancellationToken);
        }

        public Task<IGoogleAuthenticatorFactor> AddAsync(
            GoogleAuthenticatorFactorCreationOptions options,
            CancellationToken cancellationToken)
        {
            var googleAuthFactorCreationData = new GoogleAuthenticatorFactorCreationData
            {
                AccountName = options.AccountName,
                Issuer = options.Issuer,
                Status = options.Status
            };

            var parameters = new Dictionary<string, string>();
            if (options.Challenge)
            {
                parameters.Add("challenge", "true");
            }

            var creationOptions = new DefaultCreationOptions(parameters);

            return (_dataStore as IInternalAsyncDataStore).CreateAsync<GoogleAuthenticatorFactorCreationData, IGoogleAuthenticatorFactor>(
                _collectionHref,
                googleAuthFactorCreationData,
                creationOptions,
                null,
                cancellationToken);
        }
    }
}
