namespace Stormpath.SDK.Account
{
    public sealed class GoogleAuthenticatorFactorCreationOptions
    {
        // TODO check all default values

        public bool Challenge { get; set; }

        public string AccountName { get; set; }

        public string Issuer { get; set; }

        public FactorStatus Status { get; set; } = FactorStatus.Enabled;
    }
}
