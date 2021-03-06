﻿using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal partial class DefaultFactor : AbstractInstanceResource, IFactor, IFactorSync
    {
        private const string TypePropertyName = "type";
        private const string StatusPropertyName = "status";
        private const string VerificationStatusPropertyName = "verificationStatus";
        private const string AccountPropertyName = "account";
        private const string MostRecentChallengePropertyName = "mostRecentChallenge";
        private const string ChallengesPropertyName = "challenges";

        public DefaultFactor(ResourceData data)
            : base(data)
        {
        }

        public FactorType Type
        {
            get { return GetEnumProperty<FactorType>(TypePropertyName); }
            set { SetProperty(TypePropertyName, value); }
        }

        public FactorStatus Status
        {
            get { return GetEnumProperty<FactorStatus>(StatusPropertyName); }
            set { SetProperty(StatusPropertyName, value); }
        }

        public Task<IAccount> GetAccountAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().GetResourceAsync<IAccount>(
                GetLinkProperty(AccountPropertyName).Href,
                cancellationToken);

        public Task<IChallenge> GetMostRecentChallengeAsync(CancellationToken cancellationToken)
        {
            var link = GetLinkProperty(MostRecentChallengePropertyName);

            return string.IsNullOrEmpty(link?.Href)
                ? Task.FromResult<IChallenge>(null) 
                : GetInternalAsyncDataStore().GetResourceAsync<IChallenge>(link.Href, cancellationToken);
        }

        public IChallengeCollection Challenges
            => new DefaultChallengeCollection(GetLinkProperty(ChallengesPropertyName).Href, GetInternalAsyncDataStore());

        public FactorVerificationStatus VerificationStatus
        {
            get { return GetEnumProperty<FactorVerificationStatus>(VerificationStatusPropertyName); }
            set { SetProperty(VerificationStatusPropertyName, value); }
        }

        public Task<IFactor> SaveAsync(CancellationToken cancellationToken)
            => SaveAsync<IFactor>(cancellationToken);

        public Task<bool> DeleteAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);

        // Sync methods implemented in DefaultFactor.Sync.cs
    }
}