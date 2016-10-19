namespace Stormpath.SDK.Account
{
    public sealed class SmsFactorCreationOptions
    {
        public bool Challenge { get; set; } = false;

        public string Number { get; set; }

        public FactorStatus Status { get; set; }
    }
}
