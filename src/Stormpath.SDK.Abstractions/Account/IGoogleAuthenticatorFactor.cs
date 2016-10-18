namespace Stormpath.SDK.Account
{
    public interface IGoogleAuthenticatorFactor : IFactor
    {
        string AccountName { get; set; }

        string Issuer { get; set; }

        string Secret { get; }

        string KeyUri { get; }

        string Base64QrImage { get; }
    }
}
