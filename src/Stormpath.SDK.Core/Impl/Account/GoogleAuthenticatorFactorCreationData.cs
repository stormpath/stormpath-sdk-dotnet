using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class GoogleAuthenticatorFactorCreationData
    {
        public string Type { get; } = "google-authenticator";

        public string AccountName { get; set; }

        public string Issuer { get; set; }

        public FactorStatus Status { get; set; }
    }
}
