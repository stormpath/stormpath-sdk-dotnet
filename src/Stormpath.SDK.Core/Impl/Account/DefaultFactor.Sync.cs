using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    internal partial class DefaultFactor
    {
        public IAccount GetAccount()
            => GetInternalSyncDataStore().GetResource<IAccount>(
                GetLinkProperty(AccountPropertyName).Href);

        public IChallenge GetMostRecentChallenge()
        {
            var link = GetLinkProperty(MostRecentChallengePropertyName);

            return string.IsNullOrEmpty(link?.Href)
                ? null
                : GetInternalSyncDataStore().GetResource<IChallenge>(link.Href);
        }

        public IFactor Save() => Save<IFactor>();

        public bool Delete() => GetInternalSyncDataStore().Delete(this);
    }
}
