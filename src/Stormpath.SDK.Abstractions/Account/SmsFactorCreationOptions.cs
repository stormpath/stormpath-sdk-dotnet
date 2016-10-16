namespace Stormpath.SDK.Account
{
    public sealed class SmsFactorCreationOptions
    {
        // TODO check all default values

        public bool Challenge { get; set; }

        public string Number { get; set; }

        public FactorStatus Status { get; set; } = FactorStatus.Enabled;
    }
}
