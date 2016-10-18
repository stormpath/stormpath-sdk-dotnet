using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal class DefaultFactor : AbstractInstanceResource, IFactor
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

        public string Type
        {
            get { return GetStringProperty(TypePropertyName); }
            set { SetProperty(TypePropertyName, value); }
        }

        public FactorStatus Status
        {
            // TODO get status from string
            get { return GetStatusProperty<FactorStatus>(StatusPropertyName); }
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
        {
            get
            {
                // todo
                throw new NotImplementedException();
            }
        }

        public FactorVerificationStatus VerificationStatus
        {
            get { return GetProperty<FactorVerificationStatus>(VerificationStatusPropertyName); }
            set { SetProperty(VerificationStatusPropertyName, value); }
        }

        public Task<IFactor> SaveAsync(CancellationToken cancellationToken)
            => SaveAsync<IFactor>(cancellationToken);

        public Task<bool> DeleteAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().DeleteAsync(this, cancellationToken);
    }
}