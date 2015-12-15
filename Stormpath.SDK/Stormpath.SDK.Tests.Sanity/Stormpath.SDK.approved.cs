[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("DynamicProxyGenAssembly2")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Stormpath.SDK.RestSharpClient")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Stormpath.SDK.Tests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("Stormpath.SDK.Tests.Common")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.InteropServices.GuidAttribute("79a65c37-9db1-413a-ac23-708404530295")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.5", FrameworkDisplayName=".NET Framework 4.5")]

namespace Stormpath.SDK.Account
{
    
    public sealed class AccountCreationOptionsBuilder
    {
        public AccountCreationOptionsBuilder() { }
        public System.Nullable<bool> RegistrationWorkflowEnabled { get; set; }
        public Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Account.IAccount> ResponseOptions { get; }
        public Stormpath.SDK.Account.IAccountCreationOptions Build() { }
    }
    public sealed class AccountStatus : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Account.AccountStatus Disabled;
        public static Stormpath.SDK.Account.AccountStatus Enabled;
        public static Stormpath.SDK.Account.AccountStatus Unverified;
        public static Stormpath.SDK.Account.AccountStatus Parse(string status) { }
    }
    public sealed class EmailVerificationRequestBuilder
    {
        public Stormpath.SDK.AccountStore.IAccountStore AccountStore { get; set; }
        public string Login { get; set; }
    }
    public interface IAccount : Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IExtendable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Account.IAccount>, Stormpath.SDK.Resource.ISaveableWithOptions<Stormpath.SDK.Account.IAccount>, Stormpath.SDK.Tenant.IHasTenant
    {
        string Email { get; }
        Stormpath.SDK.Account.IEmailVerificationToken EmailVerificationToken { get; }
        string FullName { get; }
        string GivenName { get; }
        string MiddleName { get; }
        Stormpath.SDK.Account.AccountStatus Status { get; }
        string Surname { get; }
        string Username { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroupMembership> AddGroupAsync(Stormpath.SDK.Group.IGroup group, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroupMembership> AddGroupAsync(string hrefOrName, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> GetApplications();
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> GetDirectoryAsync(System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetGroupMemberships();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups();
        System.Threading.Tasks.Task<Stormpath.SDK.Provider.IProviderData> GetProviderDataAsync(System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<bool> IsMemberOfGroupAsync(string hrefOrName, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<bool> RemoveGroupAsync(Stormpath.SDK.Group.IGroup group, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<bool> RemoveGroupAsync(string hrefOrName, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Account.IAccount SetEmail(string email);
        Stormpath.SDK.Account.IAccount SetGivenName(string givenName);
        Stormpath.SDK.Account.IAccount SetMiddleName(string middleName);
        Stormpath.SDK.Account.IAccount SetPassword(string password);
        Stormpath.SDK.Account.IAccount SetStatus(Stormpath.SDK.Account.AccountStatus status);
        Stormpath.SDK.Account.IAccount SetSurname(string surname);
        Stormpath.SDK.Account.IAccount SetUsername(string username);
    }
    public interface IAccountCreationActions
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> CreateAccountAsync(Stormpath.SDK.Account.IAccount account, System.Action<Stormpath.SDK.Account.AccountCreationOptionsBuilder> creationOptionsAction, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> CreateAccountAsync(Stormpath.SDK.Account.IAccount account, Stormpath.SDK.Account.IAccountCreationOptions creationOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> CreateAccountAsync(Stormpath.SDK.Account.IAccount account, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> CreateAccountAsync(string givenName, string surname, string email, string password, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> CreateAccountAsync(string givenName, string surname, string email, string password, object customData, System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IAccountCreationOptions : Stormpath.SDK.Resource.ICreationOptions
    {
        System.Nullable<bool> RegistrationWorkflowEnabled { get; }
    }
    public interface IEmailVerificationRequest : Stormpath.SDK.Resource.IResource
    {
        string Login { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.AccountStore.IAccountStore> GetAccountStoreAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IEmailVerificationToken : Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Account.IEmailVerificationToken>
    {
        string GetValue();
    }
    public interface IPasswordResetToken : Stormpath.SDK.Resource.IResource
    {
        string Email { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> GetAccountAsync(System.Threading.CancellationToken cancellationToken = null);
        string GetValue();
    }
}
namespace Stormpath.SDK.AccountStore
{
    
    public interface IAccountStore : Stormpath.SDK.Resource.IResource, Stormpath.SDK.Tenant.IHasTenant
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> GetAccounts();
    }
    public interface IAccountStoreContainer<TMapping> : Stormpath.SDK.Resource.IResource, Stormpath.SDK.Tenant.IHasTenant
        where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<>, Stormpath.SDK.Resource.ISaveable<>
    {
        System.Threading.Tasks.Task<TMapping> AddAccountStoreAsync(Stormpath.SDK.AccountStore.IAccountStore accountStore, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<TMapping> AddAccountStoreAsync(string hrefOrName, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<TMapping> AddAccountStoreAsync<TSource>(System.Func<Stormpath.SDK.Linq.IAsyncQueryable<TSource>, Stormpath.SDK.Linq.IAsyncQueryable<TSource>> query, System.Threading.CancellationToken cancellationToken = null)
            where TSource : Stormpath.SDK.AccountStore.IAccountStore;
        System.Threading.Tasks.Task<TMapping> CreateAccountStoreMappingAsync(TMapping mapping, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<TMapping> GetAccountStoreMappings();
        System.Threading.Tasks.Task<Stormpath.SDK.AccountStore.IAccountStore> GetDefaultAccountStoreAsync(System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.AccountStore.IAccountStore> GetDefaultGroupStoreAsync(System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task SetDefaultAccountStoreAsync(Stormpath.SDK.AccountStore.IAccountStore accountStore, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task SetDefaultGroupStoreAsync(Stormpath.SDK.AccountStore.IAccountStore accountStore, System.Threading.CancellationToken cancellationToken = null);
    }
    [System.ObsoleteAttribute("This interface will be removed in 1.0. Use IApplicationAccountStoreMapping instea" +
        "d.")]
    public interface IAccountStoreMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<Stormpath.SDK.Application.IApplicationAccountStoreMapping>, Stormpath.SDK.Application.IApplicationAccountStoreMapping, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> { }
    public interface IAccountStoreMapping<T> : Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<T>
        where T : Stormpath.SDK.AccountStore.IAccountStoreMapping<>, Stormpath.SDK.Resource.ISaveable<>
    {
        bool IsDefaultAccountStore { get; }
        bool IsDefaultGroupStore { get; }
        int ListIndex { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.AccountStore.IAccountStore> GetAccountStoreAsync(System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> GetApplicationAsync(System.Threading.CancellationToken cancellationToken = null);
        T SetAccountStore(Stormpath.SDK.AccountStore.IAccountStore accountStore);
        T SetApplication(Stormpath.SDK.Application.IApplication application);
        T SetDefaultAccountStore(bool defaultAccountStore);
        T SetDefaultGroupStore(bool defaultGroupStore);
        T SetListIndex(int listIndex);
    }
}
namespace Stormpath.SDK.Api
{
    
    public sealed class ClientApiKeys
    {
        public ClientApiKeys() { }
        public static Stormpath.SDK.Api.IClientApiKeyBuilder Builder(Stormpath.SDK.Logging.ILogger logger = null) { }
    }
    public interface IClientApiKey
    {
        string GetId();
        string GetSecret();
        bool IsValid();
    }
    public interface IClientApiKeyBuilder
    {
        Stormpath.SDK.Api.IClientApiKey Build();
        Stormpath.SDK.Api.IClientApiKeyBuilder SetFileLocation(string path);
        Stormpath.SDK.Api.IClientApiKeyBuilder SetId(string id);
        Stormpath.SDK.Api.IClientApiKeyBuilder SetIdPropertyName(string idPropertyName);
        Stormpath.SDK.Api.IClientApiKeyBuilder SetInputStream(System.IO.Stream stream);
        Stormpath.SDK.Api.IClientApiKeyBuilder SetSecret(string secret);
        Stormpath.SDK.Api.IClientApiKeyBuilder SetSecretPropertyName(string secretPropertyName);
    }
}
namespace Stormpath.SDK.Application
{
    
    public sealed class ApplicationCreationOptionsBuilder
    {
        public ApplicationCreationOptionsBuilder() { }
        public bool CreateDirectory { get; set; }
        public string DirectoryName { get; set; }
        public Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Application.IApplication> ResponseOptions { get; }
        public Stormpath.SDK.Application.IApplicationCreationOptions Build() { }
    }
    public sealed class ApplicationStatus : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Application.ApplicationStatus Disabled;
        public static Stormpath.SDK.Application.ApplicationStatus Enabled;
        public static Stormpath.SDK.Application.ApplicationStatus Parse(string status) { }
    }
    public interface IApplication : Stormpath.SDK.Account.IAccountCreationActions, Stormpath.SDK.AccountStore.IAccountStoreContainer<Stormpath.SDK.Application.IApplicationAccountStoreMapping>, Stormpath.SDK.Group.IGroupCreationActions, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IExtendable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Application.IApplication>, Stormpath.SDK.Resource.ISaveableWithOptions<Stormpath.SDK.Application.IApplication>, Stormpath.SDK.Tenant.IHasTenant
    {
        string Description { get; }
        string Name { get; }
        Stormpath.SDK.Application.ApplicationStatus Status { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Auth.IAuthenticationResult> AuthenticateAccountAsync(Stormpath.SDK.Auth.IAuthenticationRequest request, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Auth.IAuthenticationResult> AuthenticateAccountAsync(Stormpath.SDK.Auth.IAuthenticationRequest request, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Auth.IAuthenticationResult>> responseOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Auth.IAuthenticationResult> AuthenticateAccountAsync(System.Action<Stormpath.SDK.Auth.UsernamePasswordRequestBuilder> requestBuilder, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Auth.IAuthenticationResult> AuthenticateAccountAsync(System.Action<Stormpath.SDK.Auth.UsernamePasswordRequestBuilder> requestBuilder, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Auth.IAuthenticationResult>> responseOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Auth.IAuthenticationResult> AuthenticateAccountAsync(string username, string password, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Provider.IProviderAccountResult> GetAccountAsync(Stormpath.SDK.Provider.IProviderAccountRequest request, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> GetAccounts();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups();
        System.Threading.Tasks.Task<Stormpath.SDK.Oauth.IOauthPolicy> GetOauthPolicyAsync(System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.IdSite.IIdSiteAsyncCallbackHandler NewIdSiteAsyncCallbackHandler(Stormpath.SDK.Http.IHttpRequest request);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder NewIdSiteUrlBuilder();
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> ResetPasswordAsync(string token, string newPassword, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IPasswordResetToken> SendPasswordResetEmailAsync(string email, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IPasswordResetToken> SendPasswordResetEmailAsync(string email, Stormpath.SDK.AccountStore.IAccountStore accountStore, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IPasswordResetToken> SendPasswordResetEmailAsync(string email, string hrefOrNameKey, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task SendVerificationEmailAsync(System.Action<Stormpath.SDK.Account.EmailVerificationRequestBuilder> requestBuilderAction, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task SendVerificationEmailAsync(string usernameOrEmail, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Application.IApplication SetDescription(string description);
        Stormpath.SDK.Application.IApplication SetName(string name);
        Stormpath.SDK.Application.IApplication SetStatus(Stormpath.SDK.Application.ApplicationStatus status);
        System.Threading.Tasks.Task<bool> TryAuthenticateAccountAsync(string username, string password, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> VerifyPasswordResetTokenAsync(string token, System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IApplicationAccountStoreMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<Stormpath.SDK.Application.IApplicationAccountStoreMapping>, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> { }
    public interface IApplicationCreationOptions : Stormpath.SDK.Resource.ICreationOptions
    {
        bool CreateDirectory { get; }
        string DirectoryName { get; }
    }
}
namespace Stormpath.SDK
{
    
    public class static AsyncQueryableExpandExtensions
    {
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IApplicationExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Directory.IDirectory> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Directory.IDirectory> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IDirectoryExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IGroupExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganization> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganization> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IOrganizationExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IOrganizationAccountStoreMappingExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IGroupMembershipExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.AccountStore.IAccountStoreMapping> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.AccountStore.IAccountStoreMapping> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountStoreMappingExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountStoreMappingExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Tenant.ITenant> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Tenant.ITenant> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.ITenantExpandables, object>> selector) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Auth.IAuthenticationResult> Expand(this Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Auth.IAuthenticationResult> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAuthenticationResultExpandables, object>> selector) { }
    }
    public class static AsyncQueryableExtensions
    {
        public static async System.Threading.Tasks.Task<bool> AnyAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static async System.Threading.Tasks.Task<long> CountAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static async System.Threading.Tasks.Task<TSource> FirstAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static async System.Threading.Tasks.Task<TSource> FirstOrDefaultAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static System.Threading.Tasks.Task ForEachAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Action<TSource> action, System.Threading.CancellationToken cancellationToken = null) { }
        public static async System.Threading.Tasks.Task ForEachAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Func<TSource, bool> action, System.Threading.CancellationToken cancellationToken = null) { }
        public static Stormpath.SDK.Linq.IOrderedAsyncQueryable<TSource> OrderBy<TSource, TKey>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Linq.Expressions.Expression<System.Func<TSource, TKey>> keySelector) { }
        public static Stormpath.SDK.Linq.IOrderedAsyncQueryable<TSource> OrderByDescending<TSource, TKey>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Linq.Expressions.Expression<System.Func<TSource, TKey>> keySelector) { }
        public static async System.Threading.Tasks.Task<TSource> SingleAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static async System.Threading.Tasks.Task<TSource> SingleOrDefaultAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<TSource> Skip<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, int count) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<TSource> Take<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, int count) { }
        public static Stormpath.SDK.Linq.IOrderedAsyncQueryable<TSource> ThenBy<TSource, TKey>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Linq.Expressions.Expression<System.Func<TSource, TKey>> keySelector) { }
        public static Stormpath.SDK.Linq.IOrderedAsyncQueryable<TSource> ThenByDescending<TSource, TKey>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Linq.Expressions.Expression<System.Func<TSource, TKey>> keySelector) { }
        public static async System.Threading.Tasks.Task<System.Collections.Generic.List<TSource>> ToListAsync<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Threading.CancellationToken cancellationToken = null) { }
        public static Stormpath.SDK.Linq.IAsyncQueryable<TSource> Where<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, System.Linq.Expressions.Expression<System.Func<TSource, bool>> predicate) { }
    }
    public class static AsyncQueryableFilterExtensions
    {
        public static Stormpath.SDK.Linq.IAsyncQueryable<TSource> Filter<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source, string caseInsensitiveMatch)
            where TSource : Stormpath.SDK.Resource.IResource { }
    }
    public class static RetrievalOptionsExpandExtensions
    {
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Account.IAccount> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Account.IAccount> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Application.IApplication> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Application.IApplication> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IApplicationExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Directory.IDirectory> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Directory.IDirectory> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IDirectoryExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Group.IGroup> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Group.IGroup> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IGroupExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Organization.IOrganization> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Organization.IOrganization> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IOrganizationExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IOrganizationAccountStoreMappingExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Group.IGroupMembership> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Group.IGroupMembership> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IGroupMembershipExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.AccountStore.IAccountStoreMapping> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.AccountStore.IAccountStoreMapping> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountStoreMappingExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Application.IApplicationAccountStoreMapping> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Application.IApplicationAccountStoreMapping> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountStoreMappingExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Tenant.ITenant> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Tenant.ITenant> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.ITenantExpandables, object>> selector) { }
        public static Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Auth.IAuthenticationResult> Expand(this Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Auth.IAuthenticationResult> options, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAuthenticationResultExpandables, object>> selector) { }
    }
    public class static WithinExpressionExtensions
    {
        public static bool Within(this System.DateTimeOffset field, int year) { }
        public static bool Within(this System.DateTimeOffset field, int year, int month) { }
        public static bool Within(this System.DateTimeOffset field, int year, int month, int day) { }
        public static bool Within(this System.DateTimeOffset field, int year, int month, int day, int hour) { }
        public static bool Within(this System.DateTimeOffset field, int year, int month, int day, int hour, int minute) { }
        public static bool Within(this System.DateTimeOffset field, int year, int month, int day, int hour, int minute, int second) { }
    }
}
namespace Stormpath.SDK.Auth
{
    
    public interface IAuthenticationRequest : Stormpath.SDK.Auth.IAuthenticationRequest<string, string> { }
    public interface IAuthenticationRequest<P, C>
    
    
    {
        Stormpath.SDK.AccountStore.IAccountStore AccountStore { get; }
        C Credentials { get; }
        P Principals { get; }
    }
    public interface IAuthenticationResult
    {
        bool Success { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> GetAccountAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public sealed class UsernamePasswordRequestBuilder
    {
        public UsernamePasswordRequestBuilder() { }
        public Stormpath.SDK.Auth.IAuthenticationRequest Build() { }
        public Stormpath.SDK.Auth.UsernamePasswordRequestBuilder SetAccountStore(Stormpath.SDK.AccountStore.IAccountStore accountStore) { }
        public Stormpath.SDK.Auth.UsernamePasswordRequestBuilder SetAccountStore(string hrefOrNameKey) { }
        public Stormpath.SDK.Auth.UsernamePasswordRequestBuilder SetPassword(string password) { }
        public Stormpath.SDK.Auth.UsernamePasswordRequestBuilder SetUsernameOrEmail(string usernameOrEmail) { }
    }
}
namespace Stormpath.SDK.Cache
{
    
    public abstract class AbstractCacheProvider : Stormpath.SDK.Cache.IAsynchronousCacheProvider, Stormpath.SDK.Cache.ICacheProvider, Stormpath.SDK.Cache.ISynchronousCacheProvider, System.IDisposable
    {
        public AbstractCacheProvider(bool syncSupported, bool asyncSupported) { }
        public System.Nullable<System.TimeSpan> DefaultTimeToIdle { get; }
        public System.Nullable<System.TimeSpan> DefaultTimeToLive { get; }
        protected abstract Stormpath.SDK.Cache.IAsynchronousCache CreateAsyncCache(string name, System.Nullable<System.TimeSpan> ttl, System.Nullable<System.TimeSpan> tti);
        protected abstract Stormpath.SDK.Cache.ISynchronousCache CreateSyncCache(string name, System.Nullable<System.TimeSpan> ttl, System.Nullable<System.TimeSpan> tti);
        protected virtual void Dispose(bool disposing) { }
        public void Dispose() { }
        public void SetCacheConfigurations(System.Collections.Generic.ICollection<Stormpath.SDK.Cache.ICacheConfiguration> configs) { }
        public void SetDefaultTimeToIdle(System.TimeSpan defaultTimeToIdle) { }
        public void SetDefaultTimeToLive(System.TimeSpan defaultTimeToLive) { }
        protected void ThrowIfDisposed() { }
        public override string ToString() { }
    }
    public abstract class AbstractCacheProviderBuilder<T> : Stormpath.SDK.Cache.ICacheProviderBuilder
        where T : Stormpath.SDK.Cache.AbstractCacheProvider, new ()
    {
        protected AbstractCacheProviderBuilder() { }
        protected virtual Stormpath.SDK.Cache.ICacheProvider OnBuilding(T provider) { }
    }
    public class static Caches
    {
        public static Stormpath.SDK.Cache.ICacheConfigurationBuilder ForResource<T>()
            where T : Stormpath.SDK.Resource.IResource { }
        public static Stormpath.SDK.Cache.ICacheProvider NewDisabledCacheProvider() { }
        public static Stormpath.SDK.Cache.ICacheProviderBuilder NewInMemoryCacheProvider() { }
    }
    public interface IAsynchronousCache : Stormpath.SDK.Cache.ICache, System.IDisposable
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, object>> GetAsync(string key, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, object>> PutAsync(string key, System.Collections.Generic.IDictionary<string, object> value, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, object>> RemoveAsync(string key, System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IAsynchronousCacheProvider : Stormpath.SDK.Cache.ICacheProvider, System.IDisposable
    {
        Stormpath.SDK.Cache.IAsynchronousCache GetAsyncCache(string name);
    }
    public interface ICache : System.IDisposable
    {
        string Name { get; }
        System.Nullable<System.TimeSpan> TimeToIdle { get; }
        System.Nullable<System.TimeSpan> TimeToLive { get; }
    }
    public interface ICacheConfiguration
    {
        string Name { get; }
        System.Nullable<System.TimeSpan> TimeToIdle { get; }
        System.Nullable<System.TimeSpan> TimeToLive { get; }
    }
    public interface ICacheConfigurationBuilder
    {
        Stormpath.SDK.Cache.ICacheConfiguration Build();
        Stormpath.SDK.Cache.ICacheConfigurationBuilder WithTimeToIdle(System.TimeSpan tti);
        Stormpath.SDK.Cache.ICacheConfigurationBuilder WithTimeToLive(System.TimeSpan ttl);
    }
    public interface ICacheProvider : System.IDisposable
    {
        bool IsAsynchronousSupported { get; }
        bool IsSynchronousSupported { get; }
    }
    public interface ICacheProviderBuilder
    {
        Stormpath.SDK.Cache.ICacheProvider Build();
        Stormpath.SDK.Cache.ICacheProviderBuilder WithCache(Stormpath.SDK.Cache.ICacheConfigurationBuilder builder);
        Stormpath.SDK.Cache.ICacheProviderBuilder WithDefaultTimeToIdle(System.TimeSpan tti);
        Stormpath.SDK.Cache.ICacheProviderBuilder WithDefaultTimeToLive(System.TimeSpan ttl);
    }
    public interface ISynchronousCache : Stormpath.SDK.Cache.ICache, System.IDisposable
    {
        System.Collections.Generic.IDictionary<string, object> Get(string key);
        System.Collections.Generic.IDictionary<string, object> Put(string key, System.Collections.Generic.IDictionary<string, object> value);
        System.Collections.Generic.IDictionary<string, object> Remove(string key);
    }
    public interface ISynchronousCacheProvider : Stormpath.SDK.Cache.ICacheProvider, System.IDisposable
    {
        Stormpath.SDK.Cache.ISynchronousCache GetSyncCache(string name);
    }
}
namespace Stormpath.SDK.Client
{
    
    public sealed class AuthenticationScheme : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Client.AuthenticationScheme Basic;
        public static Stormpath.SDK.Client.AuthenticationScheme SAuthc1;
        public static Stormpath.SDK.Client.AuthenticationScheme Parse(string scheme) { }
    }
    public sealed class Clients
    {
        public Clients() { }
        public static Stormpath.SDK.Client.IClientBuilder Builder() { }
    }
    public interface IClient : Stormpath.SDK.DataStore.IDataStore, Stormpath.SDK.Tenant.ITenantActions, System.IDisposable
    {
        Stormpath.SDK.Cache.ICacheProvider GetCacheProvider();
        System.Threading.Tasks.Task<Stormpath.SDK.Tenant.ITenant> GetCurrentTenantAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IClientBuilder : Stormpath.SDK.Logging.ILoggerConsumer<Stormpath.SDK.Client.IClientBuilder>, Stormpath.SDK.Serialization.ISerializerConsumer<Stormpath.SDK.Client.IClientBuilder>
    {
        Stormpath.SDK.Client.IClient Build();
        Stormpath.SDK.Client.IClientBuilder SetApiKey(Stormpath.SDK.Api.IClientApiKey apiKey);
        Stormpath.SDK.Client.IClientBuilder SetAuthenticationScheme(Stormpath.SDK.Client.AuthenticationScheme scheme);
        Stormpath.SDK.Client.IClientBuilder SetBaseUrl(string baseUrl);
        Stormpath.SDK.Client.IClientBuilder SetCacheProvider(Stormpath.SDK.Cache.ICacheProvider cacheProvider);
        Stormpath.SDK.Client.IClientBuilder SetConnectionTimeout(int timeout);
        Stormpath.SDK.Client.IClientBuilder SetHttpClient(Stormpath.SDK.Http.IHttpClient httpClient);
        Stormpath.SDK.Client.IClientBuilder SetProxy(System.Net.IWebProxy proxy);
    }
}
namespace Stormpath.SDK.CustomData
{
    
    public interface ICustomData : Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.CustomData.ICustomData>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable
    {
        int Count { get; }
        object this[string key] { get; set; }
        void Clear();
        bool ContainsKey(string key);
        object Get(string key);
        bool IsEmptyOrDefault();
        void Put(string key, object value);
        void Put(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> values);
        void Put(object customData);
        void Put(System.Collections.Generic.KeyValuePair<string, object> item);
        object Remove(string key);
        bool TryGetValue(string key, out object value);
    }
    public interface ICustomDataProxy
    {
        void Clear();
        void Put(string key, object value);
        void Put(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> values);
        void Put(System.Collections.Generic.KeyValuePair<string, object> item);
        void Put(object customData);
        void Remove(string key);
    }
}
namespace Stormpath.SDK.DataStore
{
    
    public interface IDataStore : System.IDisposable
    {
        System.Threading.Tasks.Task<T> GetResourceAsync<T>(string href, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<T> GetResourceAsync<T>(string href, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<T>> responseOptions, System.Threading.CancellationToken cancellationToken = null);
        T Instantiate<T>()
            where T : Stormpath.SDK.Resource.IResource;
    }
}
namespace Stormpath.SDK.Directory
{
    
    public sealed class DirectoryCreationOptionsBuilder
    {
        public DirectoryCreationOptionsBuilder() { }
        public Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Directory.IDirectory> ResponseOptions { get; }
        public Stormpath.SDK.Directory.IDirectoryCreationOptions Build() { }
        public Stormpath.SDK.Directory.DirectoryCreationOptionsBuilder ForProvider(Stormpath.SDK.Provider.ICreateProviderRequest request) { }
    }
    public sealed class DirectoryStatus : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Directory.DirectoryStatus Disabled;
        public static Stormpath.SDK.Directory.DirectoryStatus Enabled;
        public static Stormpath.SDK.Directory.DirectoryStatus Parse(string status) { }
    }
    public interface IDirectory : Stormpath.SDK.Account.IAccountCreationActions, Stormpath.SDK.AccountStore.IAccountStore, Stormpath.SDK.Group.IGroupCreationActions, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IExtendable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Directory.IDirectory>, Stormpath.SDK.Resource.ISaveableWithOptions<Stormpath.SDK.Directory.IDirectory>, Stormpath.SDK.Tenant.IHasTenant
    {
        string Description { get; }
        string Name { get; }
        Stormpath.SDK.Directory.DirectoryStatus Status { get; }
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> GetApplications();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups();
        System.Threading.Tasks.Task<Stormpath.SDK.Provider.IProvider> GetProviderAsync(System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Directory.IDirectory SetDescription(string description);
        Stormpath.SDK.Directory.IDirectory SetName(string name);
        Stormpath.SDK.Directory.IDirectory SetStatus(Stormpath.SDK.Directory.DirectoryStatus status);
    }
    public interface IDirectoryCreationOptions : Stormpath.SDK.Resource.ICreationOptions
    {
        Stormpath.SDK.Provider.IProvider Provider { get; }
    }
}
namespace Stormpath.SDK.Error
{
    
    public interface IError
    {
        int Code { get; }
        string DeveloperMessage { get; }
        int HttpStatus { get; }
        string Message { get; }
        string MoreInfo { get; }
    }
    public class ResourceException : System.ApplicationException, Stormpath.SDK.Error.IError, System.Runtime.Serialization.ISerializable
    {
        public int Code { get; }
        public string DeveloperMessage { get; }
        public int HttpStatus { get; }
        public string MoreInfo { get; }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
}
namespace Stormpath.SDK.Group
{
    
    public sealed class GroupCreationOptionsBuilder
    {
        public GroupCreationOptionsBuilder() { }
        public Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Group.IGroup> ResponseOptions { get; }
        public Stormpath.SDK.Group.IGroupCreationOptions Build() { }
    }
    public sealed class GroupStatus : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Group.GroupStatus Disabled;
        public static Stormpath.SDK.Group.GroupStatus Enabled;
        public static Stormpath.SDK.Group.GroupStatus Parse(string status) { }
    }
    public interface IGroup : Stormpath.SDK.AccountStore.IAccountStore, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IExtendable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Group.IGroup>, Stormpath.SDK.Resource.ISaveableWithOptions<Stormpath.SDK.Group.IGroup>, Stormpath.SDK.Tenant.IHasTenant
    {
        string Description { get; }
        string Name { get; }
        Stormpath.SDK.Group.GroupStatus Status { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroupMembership> AddAccountAsync(Stormpath.SDK.Account.IAccount account, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroupMembership> AddAccountAsync(string hrefOrEmailOrUsername, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetAccountMemberships();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> GetApplications();
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> GetDirectoryAsync(System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<bool> RemoveAccountAsync(Stormpath.SDK.Account.IAccount account, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<bool> RemoveAccountAsync(string hrefOrEmailOrUsername, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Group.IGroup SetDescription(string description);
        Stormpath.SDK.Group.IGroup SetName(string name);
        Stormpath.SDK.Group.IGroup SetStatus(Stormpath.SDK.Group.GroupStatus status);
    }
    public interface IGroupCreationActions
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroup> CreateGroupAsync(Stormpath.SDK.Group.IGroup group, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroup> CreateGroupAsync(Stormpath.SDK.Group.IGroup group, System.Action<Stormpath.SDK.Group.GroupCreationOptionsBuilder> creationOptionsAction, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroup> CreateGroupAsync(Stormpath.SDK.Group.IGroup group, Stormpath.SDK.Group.IGroupCreationOptions creationOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroup> CreateGroupAsync(string name, string description, System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IGroupCreationOptions : Stormpath.SDK.Resource.ICreationOptions { }
    public interface IGroupMembership : Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IResource
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> GetAccountAsync(System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroup> GetGroupAsync(System.Threading.CancellationToken cancellationToken = null);
    }
}
namespace Stormpath.SDK.Http
{
    
    public sealed class AuthorizationHeaderValue : Stormpath.SDK.Shared.ImmutableValueObject<Stormpath.SDK.Http.AuthorizationHeaderValue>
    {
        public AuthorizationHeaderValue(string scheme, string parameter) { }
        public string Parameter { get; }
        public string Scheme { get; }
        public override string ToString() { }
    }
    public sealed class CanonicalUri
    {
        public CanonicalUri(string href) { }
        public CanonicalUri(string href, Stormpath.SDK.Http.QueryString queryParams) { }
        public bool HasQuery { get; }
        public Stormpath.SDK.Http.QueryString QueryString { get; }
        public System.Uri ResourcePath { get; }
        public override string ToString() { }
        public System.Uri ToUri() { }
    }
    public sealed class HttpHeaders : System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>>>, System.Collections.IEnumerable
    {
        public HttpHeaders() { }
        public string Accept { get; set; }
        public Stormpath.SDK.Http.AuthorizationHeaderValue Authorization { get; set; }
        public System.Nullable<long> ContentLength { get; set; }
        public string ContentType { get; set; }
        public string Host { get; set; }
        public System.Uri Location { get; set; }
        public string UserAgent { get; set; }
        public void Add(string key, object value) { }
        [System.Runtime.CompilerServices.IteratorStateMachineAttribute(typeof(Stormpath.SDK.Http.HttpHeaders.<GetEnumerator>d__35))]
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IEnumerable<string>>> GetEnumerator() { }
        public T GetFirst<T>(string key) { }
    }
    public sealed class HttpMethod : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Http.HttpMethod Connect;
        public static Stormpath.SDK.Http.HttpMethod Delete;
        public static Stormpath.SDK.Http.HttpMethod Get;
        public static Stormpath.SDK.Http.HttpMethod Head;
        public static Stormpath.SDK.Http.HttpMethod Options;
        public static Stormpath.SDK.Http.HttpMethod Patch;
        public static Stormpath.SDK.Http.HttpMethod Post;
        public static Stormpath.SDK.Http.HttpMethod Put;
        public static Stormpath.SDK.Http.HttpMethod Trace;
        public Stormpath.SDK.Http.HttpMethod Clone() { }
        public static Stormpath.SDK.Http.HttpMethod Parse(string method) { }
    }
    public class static HttpRequests
    {
        public static Stormpath.SDK.Http.IHttpRequestBuilder NewRequestDescriptor() { }
    }
    public interface IAsynchronousHttpClient : Stormpath.SDK.Http.IHttpClient, System.IDisposable
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Http.IHttpResponse> ExecuteAsync(Stormpath.SDK.Http.IHttpRequest request, System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IHttpClient : System.IDisposable
    {
        string BaseUrl { get; }
        int ConnectionTimeout { get; }
        bool IsAsynchronousSupported { get; }
        bool IsSynchronousSupported { get; }
        System.Net.IWebProxy Proxy { get; }
    }
    public interface IHttpMessage
    {
        string Body { get; }
        string BodyContentType { get; }
        bool HasBody { get; }
        Stormpath.SDK.Http.HttpHeaders Headers { get; }
    }
    public interface IHttpRequest : Stormpath.SDK.Http.IHttpMessage
    {
        Stormpath.SDK.Http.CanonicalUri CanonicalUri { get; }
        Stormpath.SDK.Http.HttpMethod Method { get; }
    }
    public interface IHttpRequestBuilder
    {
        Stormpath.SDK.Http.IHttpRequest Build();
        Stormpath.SDK.Http.IHttpRequestBuilder WithBody(string body);
        Stormpath.SDK.Http.IHttpRequestBuilder WithBodyContentType(string bodyContentType);
        Stormpath.SDK.Http.IHttpRequestBuilder WithHeaders(System.Collections.Generic.IDictionary<string, object> headers);
        Stormpath.SDK.Http.IHttpRequestBuilder WithMethod(string methodName);
        Stormpath.SDK.Http.IHttpRequestBuilder WithMethod(Stormpath.SDK.Http.HttpMethod method);
        Stormpath.SDK.Http.IHttpRequestBuilder WithUri(string uri);
        Stormpath.SDK.Http.IHttpRequestBuilder WithUri(System.Uri uri);
    }
    public interface IHttpResponse : Stormpath.SDK.Http.IHttpMessage
    {
        string ResponsePhrase { get; }
        int StatusCode { get; }
        bool TransportError { get; }
    }
    public interface ISynchronousHttpClient : Stormpath.SDK.Http.IHttpClient, System.IDisposable
    {
        Stormpath.SDK.Http.IHttpResponse Execute(Stormpath.SDK.Http.IHttpRequest request);
    }
    public class QueryString : Stormpath.SDK.Shared.ImmutableValueObject<Stormpath.SDK.Http.QueryString>
    {
        public QueryString() { }
        public QueryString(System.Uri uri) { }
        public QueryString(string queryString) { }
        public string this[string parameter] { get; }
        public bool Any() { }
        public bool ContainsKey(string key) { }
        public string ToString(bool canonical) { }
        public override string ToString() { }
    }
    public class RequestException : System.ApplicationException
    {
        public RequestException(string message) { }
        public RequestException(string message, System.Exception innerException) { }
    }
}
namespace Stormpath.SDK.IdSite
{
    
    public interface IAccountResult
    {
        bool IsNewAccount { get; }
        string State { get; }
        Stormpath.SDK.IdSite.IdSiteResultStatus Status { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> GetAccountAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IAsynchronousNonceStore : Stormpath.SDK.IdSite.INonceStore
    {
        System.Threading.Tasks.Task<bool> ContainsNonceAsync(string nonce, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task PutNonceAsync(string nonce, System.Threading.CancellationToken cancellationToken = null);
    }
    public sealed class IdSiteResultStatus : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.IdSite.IdSiteResultStatus Authenticated;
        public static Stormpath.SDK.IdSite.IdSiteResultStatus Logout;
        public static Stormpath.SDK.IdSite.IdSiteResultStatus Registered;
        public static Stormpath.SDK.IdSite.IdSiteResultStatus Parse(string status) { }
    }
    public class IdSiteRuntimeException : Stormpath.SDK.Error.ResourceException { }
    public sealed class IdSiteSessionTimeoutException : Stormpath.SDK.IdSite.IdSiteRuntimeException { }
    public interface IIdSiteAsyncCallbackHandler
    {
        System.Threading.Tasks.Task<Stormpath.SDK.IdSite.IAccountResult> GetAccountResultAsync(System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.IdSite.IIdSiteAsyncCallbackHandler SetNonceStore(Stormpath.SDK.IdSite.INonceStore nonceStore);
        Stormpath.SDK.IdSite.IIdSiteAsyncCallbackHandler SetResultListener(Stormpath.SDK.IdSite.IIdSiteAsyncResultListener resultListener);
        Stormpath.SDK.IdSite.IIdSiteAsyncCallbackHandler SetResultListener(System.Func<Stormpath.SDK.IdSite.IAccountResult, System.Threading.CancellationToken, System.Threading.Tasks.Task> onRegistered = null, System.Func<Stormpath.SDK.IdSite.IAccountResult, System.Threading.CancellationToken, System.Threading.Tasks.Task> onAuthenticated = null, System.Func<Stormpath.SDK.IdSite.IAccountResult, System.Threading.CancellationToken, System.Threading.Tasks.Task> onLogout = null);
    }
    public interface IIdSiteAsyncResultListener
    {
        System.Threading.Tasks.Task OnAuthenticatedAsync(Stormpath.SDK.IdSite.IAccountResult result, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task OnLogoutAsync(Stormpath.SDK.IdSite.IAccountResult result, System.Threading.CancellationToken cancellationToken);
        System.Threading.Tasks.Task OnRegisteredAsync(Stormpath.SDK.IdSite.IAccountResult result, System.Threading.CancellationToken cancellationToken);
    }
    public interface IIdSiteSyncCallbackHandler
    {
        Stormpath.SDK.IdSite.IAccountResult GetAccountResult();
        Stormpath.SDK.IdSite.IIdSiteSyncCallbackHandler SetNonceStore(Stormpath.SDK.IdSite.INonceStore nonceStore);
        Stormpath.SDK.IdSite.IIdSiteSyncCallbackHandler SetResultListener(Stormpath.SDK.IdSite.IIdSiteSyncResultListener resultListener);
        Stormpath.SDK.IdSite.IIdSiteSyncCallbackHandler SetResultListener(System.Action<Stormpath.SDK.IdSite.IAccountResult> onRegistered = null, System.Action<Stormpath.SDK.IdSite.IAccountResult> onAuthenticated = null, System.Action<Stormpath.SDK.IdSite.IAccountResult> onLogout = null);
    }
    public interface IIdSiteSyncResultListener
    {
        void OnAuthenticated(Stormpath.SDK.IdSite.IAccountResult result);
        void OnLogout(Stormpath.SDK.IdSite.IAccountResult result);
        void OnRegistered(Stormpath.SDK.IdSite.IAccountResult result);
    }
    public interface IIdSiteUrlBuilder
    {
        string Build();
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder ForLogout();
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetCallbackUri(string callbackUri);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetCallbackUri(System.Uri callbackUri);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetOrganizationNameKey(string organizationNameKey);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetPath(string path);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetShowOrganizationField(bool showOrganizationField);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetState(string state);
        Stormpath.SDK.IdSite.IIdSiteUrlBuilder SetUseSubdomain(bool useSubdomain);
    }
    public interface INonce : Stormpath.SDK.Resource.IResource
    {
        string Value { get; }
    }
    public interface INonceStore
    {
        bool IsAsynchronousSupported { get; }
        bool IsSynchronousSupported { get; }
    }
    public sealed class InvalidIdSiteTokenException : Stormpath.SDK.IdSite.IdSiteRuntimeException { }
    public interface ISynchronousNonceStore : Stormpath.SDK.IdSite.INonceStore
    {
        bool ContainsNonce(string nonce);
        void PutNonce(string nonce);
    }
}
namespace Stormpath.SDK.Jwt
{
    
    public interface IJwtClaims
    {
        string Audience { get; }
        System.Nullable<System.DateTimeOffset> Expiration { get; }
        string Id { get; }
        System.Nullable<System.DateTimeOffset> IssuedAt { get; }
        string Issuer { get; }
        System.Nullable<System.DateTimeOffset> NotBefore { get; }
        string Subject { get; }
        System.Collections.Generic.IDictionary<string, object> ToDictionary();
    }
    public interface IJwtClaimsBuilder
    {
        Stormpath.SDK.Jwt.IJwtClaims Build();
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetAudience(string aud);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetClaim(string claimName, object value);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetExpiration(System.Nullable<System.DateTimeOffset> exp);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetId(string jti);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetIssuedAt(System.Nullable<System.DateTimeOffset> iat);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetIssuer(string iss);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetNotBeforeDate(System.Nullable<System.DateTimeOffset> nbf);
        Stormpath.SDK.Jwt.IJwtClaimsBuilder SetSubject(string sub);
    }
    public sealed class InvalidJwtException : System.ApplicationException
    {
        public static Stormpath.SDK.Jwt.InvalidJwtException AlreadyUsed;
        public static Stormpath.SDK.Jwt.InvalidJwtException Expired;
        public static Stormpath.SDK.Jwt.InvalidJwtException InvalidValue;
        public static Stormpath.SDK.Jwt.InvalidJwtException JwtRequired;
        public static Stormpath.SDK.Jwt.InvalidJwtException ResponseInvalidApiKeyId;
        public static Stormpath.SDK.Jwt.InvalidJwtException ResponseMissingParameter;
        public static Stormpath.SDK.Jwt.InvalidJwtException SignatureError;
        public InvalidJwtException(string message) { }
    }
    public class static Jwts
    {
        public static Stormpath.SDK.Jwt.IJwtClaimsBuilder NewClaimsBuilder() { }
    }
}
namespace Stormpath.SDK.Linq.Expandables
{
    
    public interface IAccountExpandables : Stormpath.SDK.Linq.Expandables.IExpandableApplications, Stormpath.SDK.Linq.Expandables.IExpandableCustomData, Stormpath.SDK.Linq.Expandables.IExpandableDirectory, Stormpath.SDK.Linq.Expandables.IExpandableGroupMemberships, Stormpath.SDK.Linq.Expandables.IExpandableGroups, Stormpath.SDK.Linq.Expandables.IExpandableTenant
    {
        Stormpath.SDK.Provider.IProviderData GetProviderData();
    }
    public interface IAccountStoreMappingExpandables
    {
        Stormpath.SDK.AccountStore.IAccountStore GetAccountStore();
        Stormpath.SDK.Application.IApplication GetApplication();
    }
    public interface IApplicationExpandables : Stormpath.SDK.Linq.Expandables.IExpandableAccounts, Stormpath.SDK.Linq.Expandables.IExpandableCustomData, Stormpath.SDK.Linq.Expandables.IExpandableDefaultStores, Stormpath.SDK.Linq.Expandables.IExpandableGroups, Stormpath.SDK.Linq.Expandables.IExpandableTenant
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> GetAccountStoreMappings();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> GetAccountStoreMappings(System.Nullable<int> offset, System.Nullable<int> limit);
        Stormpath.SDK.Oauth.IOauthPolicy GetOauthPolicy();
    }
    public interface IAuthenticationResultExpandables
    {
        Stormpath.SDK.Account.IAccount GetAccount();
    }
    public interface IDirectoryExpandables : Stormpath.SDK.Linq.Expandables.IExpandableAccounts, Stormpath.SDK.Linq.Expandables.IExpandableApplications, Stormpath.SDK.Linq.Expandables.IExpandableCustomData, Stormpath.SDK.Linq.Expandables.IExpandableGroups, Stormpath.SDK.Linq.Expandables.IExpandableTenant
    {
        Stormpath.SDK.Provider.IProvider GetProvider();
    }
    public interface IExpandableAccounts
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> GetAccounts();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> GetAccounts(System.Nullable<int> offset, System.Nullable<int> limit);
    }
    public interface IExpandableApplications
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> GetApplications();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> GetApplications(System.Nullable<int> offset, System.Nullable<int> limit);
    }
    public interface IExpandableCustomData
    {
        Stormpath.SDK.CustomData.ICustomData GetCustomData();
    }
    public interface IExpandableDefaultStores
    {
        Stormpath.SDK.AccountStore.IAccountStore GetDefaultAccountStore();
        Stormpath.SDK.AccountStore.IAccountStore GetDefaultGroupStore();
    }
    public interface IExpandableDirectory
    {
        Stormpath.SDK.Directory.IDirectory GetDirectory();
    }
    public interface IExpandableGroupMemberships
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetGroupMemberships();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetGroupMemberships(System.Nullable<int> offset, System.Nullable<int> limit);
    }
    public interface IExpandableGroups
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups(System.Nullable<int> offset, System.Nullable<int> limit);
    }
    public interface IExpandableTenant
    {
        Stormpath.SDK.Tenant.ITenant GetTenant();
    }
    public interface IGroupExpandables : Stormpath.SDK.Linq.Expandables.IExpandableAccounts, Stormpath.SDK.Linq.Expandables.IExpandableApplications, Stormpath.SDK.Linq.Expandables.IExpandableCustomData, Stormpath.SDK.Linq.Expandables.IExpandableDirectory, Stormpath.SDK.Linq.Expandables.IExpandableTenant
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetAccountMemberships();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetAccountMemberships(System.Nullable<int> offset, System.Nullable<int> limit);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.AccountStore.IAccountStoreMapping> GetApplicationMappings();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroupMembership> GetApplicationMappings(System.Nullable<int> offset, System.Nullable<int> limit);
    }
    public interface IGroupMembershipExpandables
    {
        Stormpath.SDK.Account.IAccount GetAccount();
        Stormpath.SDK.Group.IGroup GetGroup();
    }
    public interface IOrganizationAccountStoreMappingExpandables
    {
        Stormpath.SDK.AccountStore.IAccountStore GetAccountStore();
        Stormpath.SDK.Organization.IOrganization GetOrganization();
    }
    public interface IOrganizationExpandables : Stormpath.SDK.Linq.Expandables.IExpandableAccounts, Stormpath.SDK.Linq.Expandables.IExpandableCustomData, Stormpath.SDK.Linq.Expandables.IExpandableDefaultStores, Stormpath.SDK.Linq.Expandables.IExpandableGroups, Stormpath.SDK.Linq.Expandables.IExpandableTenant
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> GetAccountStoreMappings();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> GetAccountStoreMappings(System.Nullable<int> offset, System.Nullable<int> limit);
    }
    public interface ITenantExpandables : Stormpath.SDK.Linq.Expandables.IExpandableAccounts, Stormpath.SDK.Linq.Expandables.IExpandableApplications, Stormpath.SDK.Linq.Expandables.IExpandableCustomData, Stormpath.SDK.Linq.Expandables.IExpandableGroups
    {
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Directory.IDirectory> GetDirectories();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Directory.IDirectory> GetDirectories(System.Nullable<int> offset, System.Nullable<int> limit);
    }
}
namespace Stormpath.SDK.Linq
{
    
    public interface IAsyncQueryable<T>
    
    {
        System.Collections.Generic.IEnumerable<T> CurrentPage { get; }
        System.Linq.Expressions.Expression Expression { get; }
        Stormpath.SDK.Linq.IAsyncQueryProvider<T> Provider { get; }
        System.Threading.Tasks.Task<bool> MoveNextAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IAsyncQueryProvider<T>
    
    {
        Stormpath.SDK.Linq.IAsyncQueryable<T> CreateQuery(System.Linq.Expressions.Expression expression);
    }
    public interface IOrderedAsyncQueryable<T> : Stormpath.SDK.Linq.IAsyncQueryable<T> { }
}
namespace Stormpath.SDK.Logging
{
    
    public interface ILogger
    {
        void Log(Stormpath.SDK.Logging.LogEntry entry);
    }
    public interface ILoggerConsumer<out T>
    
    {
        T SetLogger(Stormpath.SDK.Logging.ILogger logger);
    }
    public sealed class InMemoryLogger : Stormpath.SDK.Logging.ILogger
    {
        public InMemoryLogger() { }
        public override string ToString() { }
    }
    public sealed class LogEntry
    {
        public readonly System.Exception Exception;
        public readonly string Message;
        public readonly Stormpath.SDK.Logging.LogLevel Severity;
        public readonly string Source;
        public LogEntry(Stormpath.SDK.Logging.LogLevel severity, string message, string source, System.Exception exception) { }
    }
    public class static LoggerExtensions
    {
        public static void Error(this Stormpath.SDK.Logging.ILogger logger, string message, string source = null) { }
        public static void Error(this Stormpath.SDK.Logging.ILogger logger, System.Exception exception, string message = null, string source = null) { }
        public static void Fatal(this Stormpath.SDK.Logging.ILogger logger, string message, string source = null) { }
        public static void Fatal(this Stormpath.SDK.Logging.ILogger logger, System.Exception exception, string message = null, string source = null) { }
        public static void Info(this Stormpath.SDK.Logging.ILogger logger, string message, string source = null) { }
        public static void Trace(this Stormpath.SDK.Logging.ILogger logger, string message, string source = null) { }
        public static void Warn(this Stormpath.SDK.Logging.ILogger logger, string message, string source = null) { }
        public static void Warn(this Stormpath.SDK.Logging.ILogger logger, System.Exception exception, string message = null, string source = null) { }
    }
    public enum LogLevel
    {
        Trace = 0,
        Info = 1,
        Warn = 2,
        Error = 3,
        Fatal = 4,
    }
}
namespace Stormpath.SDK.Oauth
{
    
    public interface IOauthPolicy : Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Oauth.IOauthPolicy>, Stormpath.SDK.Tenant.IHasTenant
    {
        System.TimeSpan AccessTokenTimeToLive { get; }
        System.TimeSpan RefreshTokenTimeToLive { get; }
        string TokenEndpointHref { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> GetApplicationAsync(System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Oauth.IOauthPolicy SetAccessTokenTimeToLive(System.TimeSpan accessTokenTtl);
        Stormpath.SDK.Oauth.IOauthPolicy SetAccessTokenTimeToLive(string accessTokenTtl);
        Stormpath.SDK.Oauth.IOauthPolicy SetRefreshTokenTimeToLive(System.TimeSpan refreshTokenTtl);
        Stormpath.SDK.Oauth.IOauthPolicy SetRefreshTokenTimeToLive(string refreshTokenTtl);
    }
}
namespace Stormpath.SDK.Organization
{
    
    public interface IOrganization : Stormpath.SDK.Account.IAccountCreationActions, Stormpath.SDK.AccountStore.IAccountStore, Stormpath.SDK.AccountStore.IAccountStoreContainer<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping>, Stormpath.SDK.Group.IGroupCreationActions, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IExtendable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Organization.IOrganization>, Stormpath.SDK.Resource.ISaveableWithOptions<Stormpath.SDK.Organization.IOrganization>, Stormpath.SDK.Tenant.IHasTenant
    {
        string Description { get; }
        string Name { get; }
        string NameKey { get; }
        Stormpath.SDK.Organization.OrganizationStatus Status { get; }
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups();
        Stormpath.SDK.Organization.IOrganization SetDescription(string description);
        Stormpath.SDK.Organization.IOrganization SetName(string name);
        Stormpath.SDK.Organization.IOrganization SetNameKey(string nameKey);
        Stormpath.SDK.Organization.IOrganization SetStatus(Stormpath.SDK.Organization.OrganizationStatus status);
    }
    public interface IOrganizationAccountStoreMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping>, Stormpath.SDK.Resource.IDeletable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Resource.ISaveable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping>
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Organization.IOrganization> GetOrganizationAsync(System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Organization.IOrganizationAccountStoreMapping SetOrganization(Stormpath.SDK.Organization.IOrganization organization);
    }
    public interface IOrganizationCreationOptions : Stormpath.SDK.Resource.ICreationOptions
    {
        bool CreateDirectory { get; }
        string DirectoryName { get; }
    }
    public sealed class OrganizationCreationOptionsBuilder
    {
        public OrganizationCreationOptionsBuilder() { }
        public bool CreateDirectory { get; set; }
        public string DirectoryName { get; set; }
        public Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Organization.IOrganization> ResponseOptions { get; }
        public Stormpath.SDK.Organization.IOrganizationCreationOptions Build() { }
    }
    public sealed class OrganizationStatus : Stormpath.SDK.Shared.StringEnumeration
    {
        public static Stormpath.SDK.Organization.OrganizationStatus Disabled;
        public static Stormpath.SDK.Organization.OrganizationStatus Enabled;
        public static Stormpath.SDK.Organization.OrganizationStatus Parse(string status) { }
    }
}
namespace Stormpath.SDK.Provider
{
    
    public class static ClientProviderExtensions
    {
        public static Stormpath.SDK.Provider.IProviderFactory Providers(this Stormpath.SDK.Client.IClient client) { }
    }
    public interface ICreateProviderRequest
    {
        Stormpath.SDK.Provider.IProvider GetProvider();
    }
    public interface ICreateProviderRequestBuilder<T>
        where T : Stormpath.SDK.Provider.ICreateProviderRequestBuilder<>
    {
        Stormpath.SDK.Provider.ICreateProviderRequest Build();
        T SetClientId(string clientId);
        T SetClientSecret(string clientSecret);
    }
    public interface IFacebookAccountRequestBuilder : Stormpath.SDK.Provider.IProviderAccountRequestBuilder<Stormpath.SDK.Provider.IFacebookAccountRequestBuilder> { }
    public interface IFacebookCreateProviderRequestBuilder : Stormpath.SDK.Provider.ICreateProviderRequestBuilder<Stormpath.SDK.Provider.IFacebookCreateProviderRequestBuilder> { }
    public interface IFacebookProvider : Stormpath.SDK.Provider.IProvider, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
    public interface IFacebookProviderData : Stormpath.SDK.Provider.IProviderData, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string AccessToken { get; }
    }
    public interface IFacebookRequestFactory : Stormpath.SDK.Provider.IProviderRequestFactory<Stormpath.SDK.Provider.IFacebookAccountRequestBuilder, Stormpath.SDK.Provider.IFacebookCreateProviderRequestBuilder> { }
    public interface IGithubAccountRequestBuilder : Stormpath.SDK.Provider.IProviderAccountRequestBuilder<Stormpath.SDK.Provider.IGithubAccountRequestBuilder> { }
    public interface IGithubCreateProviderRequestBuilder : Stormpath.SDK.Provider.ICreateProviderRequestBuilder<Stormpath.SDK.Provider.IGithubCreateProviderRequestBuilder> { }
    public interface IGithubProvider : Stormpath.SDK.Provider.IProvider, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
    public interface IGithubProviderData : Stormpath.SDK.Provider.IProviderData, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string AccessToken { get; }
    }
    public interface IGithubRequestFactory : Stormpath.SDK.Provider.IProviderRequestFactory<Stormpath.SDK.Provider.IGithubAccountRequestBuilder, Stormpath.SDK.Provider.IGithubCreateProviderRequestBuilder> { }
    public interface IGoogleAccountRequestBuilder : Stormpath.SDK.Provider.IProviderAccountRequestBuilder<Stormpath.SDK.Provider.IGoogleAccountRequestBuilder>
    {
        Stormpath.SDK.Provider.IGoogleAccountRequestBuilder SetCode(string code);
    }
    public interface IGoogleCreateProviderRequestBuilder : Stormpath.SDK.Provider.ICreateProviderRequestBuilder<Stormpath.SDK.Provider.IGoogleCreateProviderRequestBuilder>
    {
        Stormpath.SDK.Provider.IGoogleCreateProviderRequestBuilder SetRedirectUri(string redirectUri);
    }
    public interface IGoogleProvider : Stormpath.SDK.Provider.IProvider, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string ClientId { get; }
        string ClientSecret { get; }
        string RedirectUri { get; }
    }
    public interface IGoogleProviderData : Stormpath.SDK.Provider.IProviderData, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string AccessToken { get; }
        string RefreshToken { get; }
    }
    public interface IGoogleRequestFactory : Stormpath.SDK.Provider.IProviderRequestFactory<Stormpath.SDK.Provider.IGoogleAccountRequestBuilder, Stormpath.SDK.Provider.IGoogleCreateProviderRequestBuilder> { }
    public interface ILinkedInAccountRequestBuilder : Stormpath.SDK.Provider.IProviderAccountRequestBuilder<Stormpath.SDK.Provider.ILinkedInAccountRequestBuilder> { }
    public interface ILinkedInCreateProviderRequestBuilder : Stormpath.SDK.Provider.ICreateProviderRequestBuilder<Stormpath.SDK.Provider.ILinkedInCreateProviderRequestBuilder> { }
    public interface ILinkedInProvider : Stormpath.SDK.Provider.IProvider, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
    public interface ILinkedInProviderData : Stormpath.SDK.Provider.IProviderData, Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string AccessToken { get; }
    }
    public interface ILinkedInRequestFactory : Stormpath.SDK.Provider.IProviderRequestFactory<Stormpath.SDK.Provider.ILinkedInAccountRequestBuilder, Stormpath.SDK.Provider.ILinkedInCreateProviderRequestBuilder> { }
    public interface IProvider : Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string ProviderId { get; }
    }
    public interface IProviderAccountRequest
    {
        Stormpath.SDK.Provider.IProviderData GetProviderData();
    }
    public interface IProviderAccountRequestBuilder<T>
        where T : Stormpath.SDK.Provider.IProviderAccountRequestBuilder<>
    {
        Stormpath.SDK.Provider.IProviderAccountRequest Build();
        T SetAccessToken(string accessToken);
    }
    public interface IProviderAccountResult : Stormpath.SDK.Resource.IResource
    {
        Stormpath.SDK.Account.IAccount Account { get; }
        bool IsNewAccount { get; }
    }
    public interface IProviderData : Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IResource
    {
        string ProviderId { get; }
    }
    public interface IProviderFactory
    {
        Stormpath.SDK.Provider.IFacebookRequestFactory Facebook();
        Stormpath.SDK.Provider.IGithubRequestFactory Github();
        Stormpath.SDK.Provider.IGoogleRequestFactory Google();
        Stormpath.SDK.Provider.ILinkedInRequestFactory LinkedIn();
    }
    public interface IProviderRequestFactory<A, P>
        where A : Stormpath.SDK.Provider.IProviderAccountRequestBuilder<>
        where P : Stormpath.SDK.Provider.ICreateProviderRequestBuilder<>
    {
        A Account();
        P Builder();
    }
}
namespace Stormpath.SDK.Resource
{
    
    public interface IAuditable
    {
        System.DateTimeOffset CreatedAt { get; }
        System.DateTimeOffset ModifiedAt { get; }
    }
    public interface ICreationOptions
    {
        string GetQueryString();
    }
    public interface IDeletable
    {
        System.Threading.Tasks.Task<bool> DeleteAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IExtendable
    {
        Stormpath.SDK.CustomData.ICustomDataProxy CustomData { get; }
        System.Threading.Tasks.Task<Stormpath.SDK.CustomData.ICustomData> GetCustomDataAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface IResource
    {
        string Href { get; }
    }
    public interface IRetrievalOptions<T> : Stormpath.SDK.Resource.ICreationOptions { }
    public interface ISaveable<T>
        where T : Stormpath.SDK.Resource.IResource
    {
        System.Threading.Tasks.Task<T> SaveAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface ISaveableWithOptions<T> : Stormpath.SDK.Resource.ISaveable<T>
        where T : Stormpath.SDK.Resource.IResource
    {
        System.Threading.Tasks.Task<T> SaveAsync(System.Action<Stormpath.SDK.Resource.IRetrievalOptions<T>> responseOptions, System.Threading.CancellationToken cancellationToken = null);
    }
}
namespace Stormpath.SDK.Serialization
{
    
    public interface IJsonSerializer
    {
        System.Collections.Generic.IDictionary<string, object> Deserialize(string json);
        string Serialize(System.Collections.Generic.IDictionary<string, object> map);
    }
    public interface ISerializerConsumer<out T>
    
    {
        T SetSerializer(Stormpath.SDK.Serialization.IJsonSerializer serializer);
    }
}
namespace Stormpath.SDK.Shared
{
    
    public abstract class ImmutableValueObject<T> : System.IEquatable<T>
        where T : Stormpath.SDK.Shared.ImmutableValueObject<>
    {
        public ImmutableValueObject() { }
        public ImmutableValueObject(System.Func<T, T, bool> equalityFunction) { }
        public override bool Equals(object obj) { }
        public virtual bool Equals(T other) { }
        public override int GetHashCode() { }
    }
    public abstract class StringEnumeration : System.IComparable
    {
        protected StringEnumeration(string value) { }
        public string Value { get; }
        public int CompareTo(object other) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public override string ToString() { }
    }
}
namespace Stormpath.SDK.Sync
{
    
    public class static SyncAccountCreationActionsExtensions
    {
        public static Stormpath.SDK.Account.IAccount CreateAccount(this Stormpath.SDK.Account.IAccountCreationActions source, Stormpath.SDK.Account.IAccount account, System.Action<Stormpath.SDK.Account.AccountCreationOptionsBuilder> creationOptionsAction) { }
        public static Stormpath.SDK.Account.IAccount CreateAccount(this Stormpath.SDK.Account.IAccountCreationActions source, Stormpath.SDK.Account.IAccount account, Stormpath.SDK.Account.IAccountCreationOptions creationOptions) { }
        public static Stormpath.SDK.Account.IAccount CreateAccount(this Stormpath.SDK.Account.IAccountCreationActions source, Stormpath.SDK.Account.IAccount account) { }
        public static Stormpath.SDK.Account.IAccount CreateAccount(this Stormpath.SDK.Account.IAccountCreationActions source, string givenName, string surname, string email, string password) { }
        public static Stormpath.SDK.Account.IAccount CreateAccount(this Stormpath.SDK.Account.IAccountCreationActions source, string givenName, string surname, string email, string password, object customData = null) { }
    }
    public class static SyncAccountExtensions
    {
        public static Stormpath.SDK.Group.IGroupMembership AddGroup(this Stormpath.SDK.Account.IAccount account, Stormpath.SDK.Group.IGroup group) { }
        public static Stormpath.SDK.Group.IGroupMembership AddGroup(this Stormpath.SDK.Account.IAccount account, string hrefOrName) { }
        public static Stormpath.SDK.Directory.IDirectory GetDirectory(this Stormpath.SDK.Account.IAccount account) { }
        public static Stormpath.SDK.Provider.IProviderData GetProviderData(this Stormpath.SDK.Account.IAccount account) { }
        public static bool IsMemberOfGroup(this Stormpath.SDK.Account.IAccount account, string hrefOrName) { }
        public static bool RemoveGroup(this Stormpath.SDK.Account.IAccount account, Stormpath.SDK.Group.IGroup group) { }
        public static bool RemoveGroup(this Stormpath.SDK.Account.IAccount account, string hrefOrName) { }
    }
    public class static SyncAccountResultExtensions
    {
        public static Stormpath.SDK.Account.IAccount GetAccount(this Stormpath.SDK.IdSite.IAccountResult source) { }
    }
    public class static SyncAccountStoreContainerExtensions
    {
        public static TMapping AddAccountStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container, Stormpath.SDK.AccountStore.IAccountStore accountStore)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static TMapping AddAccountStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container, string hrefOrName)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static TMapping CreateAccountStoreMapping<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container, TMapping mapping)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static Stormpath.SDK.AccountStore.IAccountStore GetDefaultAccountStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static Stormpath.SDK.AccountStore.IAccountStore GetDefaultGroupStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static void SetDefaultAccountStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container, Stormpath.SDK.AccountStore.IAccountStore accountStore)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static void SetDefaultGroupStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<TMapping> container, Stormpath.SDK.AccountStore.IAccountStore accountStore)
            where TMapping : Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
    }
    public class static SyncAccountStoreExtensions { }
    public class static SyncAccountStoreMappingExtensions
    {
        public static Stormpath.SDK.AccountStore.IAccountStore GetAccountStore<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreMapping<TMapping> accountStoreMapping)
            where TMapping :  class, Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
        public static Stormpath.SDK.Application.IApplication GetApplication<TMapping>(this Stormpath.SDK.AccountStore.IAccountStoreMapping<TMapping> accountStoreMapping)
            where TMapping :  class, Stormpath.SDK.AccountStore.IAccountStoreMapping<> { }
    }
    public class static SyncApplicationAccountStoreContainerExtensions
    {
        public static Stormpath.SDK.Application.IApplicationAccountStoreMapping AddAccountStore<T>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<Stormpath.SDK.Application.IApplicationAccountStoreMapping> container, System.Func<System.Linq.IQueryable<T>, System.Linq.IQueryable<T>> query)
            where T : Stormpath.SDK.AccountStore.IAccountStore { }
    }
    public class static SyncApplicationExtensions
    {
        public static Stormpath.SDK.Auth.IAuthenticationResult AuthenticateAccount(this Stormpath.SDK.Application.IApplication application, Stormpath.SDK.Auth.IAuthenticationRequest request) { }
        public static Stormpath.SDK.Auth.IAuthenticationResult AuthenticateAccount(this Stormpath.SDK.Application.IApplication application, Stormpath.SDK.Auth.IAuthenticationRequest request, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Auth.IAuthenticationResult>> responseOptions) { }
        public static Stormpath.SDK.Auth.IAuthenticationResult AuthenticateAccount(this Stormpath.SDK.Application.IApplication application, System.Action<Stormpath.SDK.Auth.UsernamePasswordRequestBuilder> requestBuilder) { }
        public static Stormpath.SDK.Auth.IAuthenticationResult AuthenticateAccount(this Stormpath.SDK.Application.IApplication application, System.Action<Stormpath.SDK.Auth.UsernamePasswordRequestBuilder> requestBuilder, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<Stormpath.SDK.Auth.IAuthenticationResult>> responseOptions) { }
        public static Stormpath.SDK.Auth.IAuthenticationResult AuthenticateAccount(this Stormpath.SDK.Application.IApplication application, string username, string password) { }
        public static Stormpath.SDK.Provider.IProviderAccountResult GetAccount(this Stormpath.SDK.Application.IApplication application, Stormpath.SDK.Provider.IProviderAccountRequest request) { }
        public static Stormpath.SDK.Oauth.IOauthPolicy GetOauthPolicy(this Stormpath.SDK.Application.IApplication application) { }
        public static Stormpath.SDK.IdSite.IIdSiteSyncCallbackHandler NewIdSiteSyncCallbackHandler(this Stormpath.SDK.Application.IApplication application, Stormpath.SDK.Http.IHttpRequest request) { }
        public static Stormpath.SDK.Account.IAccount ResetPassword(this Stormpath.SDK.Application.IApplication application, string token, string newPassword) { }
        public static Stormpath.SDK.Account.IPasswordResetToken SendPasswordResetEmail(this Stormpath.SDK.Application.IApplication application, string email) { }
        public static Stormpath.SDK.Account.IPasswordResetToken SendPasswordResetEmail(this Stormpath.SDK.Application.IApplication application, string email, Stormpath.SDK.AccountStore.IAccountStore accountStore) { }
        public static Stormpath.SDK.Account.IPasswordResetToken SendPasswordResetEmail(this Stormpath.SDK.Application.IApplication application, string email, string hrefOrNameKey) { }
        public static void SendVerificationEmail(this Stormpath.SDK.Application.IApplication application, System.Action<Stormpath.SDK.Account.EmailVerificationRequestBuilder> requestBuilderAction) { }
        public static void SendVerificationEmail(this Stormpath.SDK.Application.IApplication application, string usernameOrEmail) { }
        public static bool TryAuthenticateAccount(this Stormpath.SDK.Application.IApplication application, string username, string password) { }
        public static Stormpath.SDK.Account.IAccount VerifyPasswordResetToken(this Stormpath.SDK.Application.IApplication application, string token) { }
    }
    public class static SyncAuthenticationResultExtensions
    {
        public static Stormpath.SDK.Account.IAccount GetAccount(this Stormpath.SDK.Auth.IAuthenticationResult authResult) { }
    }
    public class static SyncClientExtensions
    {
        public static Stormpath.SDK.Tenant.ITenant GetCurrentTenant(this Stormpath.SDK.Client.IClient client) { }
    }
    public class static SyncDataStoreExtensions
    {
        public static T GetResource<T>(this Stormpath.SDK.DataStore.IDataStore dataStore, string href) { }
        public static T GetResource<T>(this Stormpath.SDK.DataStore.IDataStore dataStore, string href, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<T>> responseOptions) { }
    }
    public class static SyncDirectoryExtensions
    {
        public static Stormpath.SDK.Provider.IProvider GetProvider(this Stormpath.SDK.Directory.IDirectory directory) { }
    }
    public class static SyncEmailVerificationRequestExtensions
    {
        public static Stormpath.SDK.AccountStore.IAccountStore GetAccountStore(this Stormpath.SDK.Account.IEmailVerificationRequest request) { }
    }
    public class static SyncExpandExtensions
    {
        public static System.Linq.IQueryable<Stormpath.SDK.Account.IAccount> Expand(this System.Linq.IQueryable<Stormpath.SDK.Account.IAccount> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Application.IApplication> Expand(this System.Linq.IQueryable<Stormpath.SDK.Application.IApplication> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IApplicationExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Directory.IDirectory> Expand(this System.Linq.IQueryable<Stormpath.SDK.Directory.IDirectory> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IDirectoryExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Group.IGroup> Expand(this System.Linq.IQueryable<Stormpath.SDK.Group.IGroup> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IGroupExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Organization.IOrganization> Expand(this System.Linq.IQueryable<Stormpath.SDK.Organization.IOrganization> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IOrganizationExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> Expand(this System.Linq.IQueryable<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IOrganizationAccountStoreMappingExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Group.IGroupMembership> Expand(this System.Linq.IQueryable<Stormpath.SDK.Group.IGroupMembership> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IGroupMembershipExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.AccountStore.IAccountStoreMapping> Expand(this System.Linq.IQueryable<Stormpath.SDK.AccountStore.IAccountStoreMapping> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountStoreMappingExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> Expand(this System.Linq.IQueryable<Stormpath.SDK.Application.IApplicationAccountStoreMapping> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAccountStoreMappingExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Tenant.ITenant> Expand(this System.Linq.IQueryable<Stormpath.SDK.Tenant.ITenant> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.ITenantExpandables, object>> selector) { }
        public static System.Linq.IQueryable<Stormpath.SDK.Auth.IAuthenticationResult> Expand(this System.Linq.IQueryable<Stormpath.SDK.Auth.IAuthenticationResult> source, System.Linq.Expressions.Expression<System.Func<Stormpath.SDK.Linq.Expandables.IAuthenticationResultExpandables, object>> selector) { }
    }
    public class static SyncExtendableExtensions
    {
        public static Stormpath.SDK.CustomData.ICustomData GetCustomData(this Stormpath.SDK.Resource.IExtendable resource) { }
    }
    public class static SyncFilterExtensions
    {
        public static System.Linq.IQueryable<TSource> Filter<TSource>(this System.Linq.IQueryable<TSource> source, string caseInsensitiveMatch) { }
    }
    public class static SyncGroupCreationExtensions
    {
        public static Stormpath.SDK.Group.IGroup CreateGroup(this Stormpath.SDK.Group.IGroupCreationActions source, Stormpath.SDK.Group.IGroup group) { }
        public static Stormpath.SDK.Group.IGroup CreateGroup(this Stormpath.SDK.Group.IGroupCreationActions source, Stormpath.SDK.Group.IGroup group, System.Action<Stormpath.SDK.Group.GroupCreationOptionsBuilder> creationOptionsAction) { }
        public static Stormpath.SDK.Group.IGroup CreateGroup(this Stormpath.SDK.Group.IGroupCreationActions source, Stormpath.SDK.Group.IGroup group, Stormpath.SDK.Group.IGroupCreationOptions creationOptions) { }
        public static Stormpath.SDK.Group.IGroup CreateGroup(this Stormpath.SDK.Group.IGroupCreationActions source, string name, string description) { }
    }
    public class static SyncGroupExtensions
    {
        public static Stormpath.SDK.Group.IGroupMembership AddAccount(this Stormpath.SDK.Group.IGroup group, Stormpath.SDK.Account.IAccount account) { }
        public static Stormpath.SDK.Group.IGroupMembership AddAccount(this Stormpath.SDK.Group.IGroup group, string hrefOrEmailOrUsername) { }
        public static Stormpath.SDK.Directory.IDirectory GetDirectory(this Stormpath.SDK.Group.IGroup group) { }
        public static bool RemoveAccount(this Stormpath.SDK.Group.IGroup group, Stormpath.SDK.Account.IAccount account) { }
        public static bool RemoveAccount(this Stormpath.SDK.Group.IGroup group, string hrefOrEmailOrUsername) { }
    }
    public class static SyncGroupMembershipExtensions
    {
        public static Stormpath.SDK.Account.IAccount GetAccount(this Stormpath.SDK.Group.IGroupMembership groupMembership) { }
        public static Stormpath.SDK.Group.IGroup GetGroup(this Stormpath.SDK.Group.IGroupMembership groupMembership) { }
    }
    public class static SyncHasTenantExtensions
    {
        public static Stormpath.SDK.Tenant.ITenant GetTenant(this Stormpath.SDK.Tenant.IHasTenant resource) { }
    }
    public class static SyncOauthPolicyExtensions
    {
        public static Stormpath.SDK.Application.IApplication GetApplication(this Stormpath.SDK.Oauth.IOauthPolicy policy) { }
    }
    public class static SyncOrganizationAccountStoreContainerExtensions
    {
        public static Stormpath.SDK.Organization.IOrganizationAccountStoreMapping AddAccountStore<T>(this Stormpath.SDK.AccountStore.IAccountStoreContainer<Stormpath.SDK.Organization.IOrganizationAccountStoreMapping> container, System.Func<System.Linq.IQueryable<T>, System.Linq.IQueryable<T>> query)
            where T : Stormpath.SDK.AccountStore.IAccountStore { }
    }
    public class static SyncOrganizationAccountStoreMappingExtensions
    {
        public static Stormpath.SDK.Organization.IOrganization GetOrganization(this Stormpath.SDK.Organization.IOrganizationAccountStoreMapping accountStoreMapping) { }
    }
    public class static SyncPasswordResetTokenExtensions
    {
        public static Stormpath.SDK.Account.IAccount GetAccount(this Stormpath.SDK.Account.IPasswordResetToken token) { }
    }
    public class static SyncQueryableExtensions
    {
        public static System.Linq.IQueryable<TSource> Synchronously<TSource>(this Stormpath.SDK.Linq.IAsyncQueryable<TSource> source) { }
    }
    public class static SyncResourceExtensions
    {
        public static bool Delete(this Stormpath.SDK.Resource.IDeletable resource) { }
        public static T Save<T>(this Stormpath.SDK.Resource.ISaveable<T> resource)
            where T : Stormpath.SDK.Resource.IResource { }
        public static T Save<T>(this Stormpath.SDK.Resource.ISaveableWithOptions<T> resource, System.Action<Stormpath.SDK.Resource.IRetrievalOptions<T>> responseOptions)
            where T : Stormpath.SDK.Resource.IResource { }
    }
    public class static SyncTenantActionsExtensions
    {
        public static Stormpath.SDK.Application.IApplication CreateApplication(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Application.IApplication application, System.Action<Stormpath.SDK.Application.ApplicationCreationOptionsBuilder> creationOptionsAction) { }
        public static Stormpath.SDK.Application.IApplication CreateApplication(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Application.IApplication application, Stormpath.SDK.Application.IApplicationCreationOptions creationOptions) { }
        public static Stormpath.SDK.Application.IApplication CreateApplication(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Application.IApplication application) { }
        public static Stormpath.SDK.Application.IApplication CreateApplication(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string name, bool createDirectory) { }
        public static Stormpath.SDK.Directory.IDirectory CreateDirectory(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Directory.IDirectory directory) { }
        public static Stormpath.SDK.Directory.IDirectory CreateDirectory(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Directory.IDirectory directory, System.Action<Stormpath.SDK.Directory.DirectoryCreationOptionsBuilder> creationOptionsAction) { }
        public static Stormpath.SDK.Directory.IDirectory CreateDirectory(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Directory.IDirectory directory, Stormpath.SDK.Directory.IDirectoryCreationOptions creationOptions) { }
        public static Stormpath.SDK.Directory.IDirectory CreateDirectory(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string name, string description, Stormpath.SDK.Directory.DirectoryStatus status) { }
        public static Stormpath.SDK.Organization.IOrganization CreateOrganization(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Organization.IOrganization organization) { }
        public static Stormpath.SDK.Organization.IOrganization CreateOrganization(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Organization.IOrganization organization, System.Action<Stormpath.SDK.Organization.OrganizationCreationOptionsBuilder> creationOptionsAction) { }
        public static Stormpath.SDK.Organization.IOrganization CreateOrganization(this Stormpath.SDK.Tenant.ITenantActions tenantActions, Stormpath.SDK.Organization.IOrganization organization, Stormpath.SDK.Organization.IOrganizationCreationOptions creationOptions) { }
        public static Stormpath.SDK.Organization.IOrganization CreateOrganization(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string name, string description) { }
        public static Stormpath.SDK.Account.IAccount GetAccount(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string href) { }
        public static Stormpath.SDK.Application.IApplication GetApplication(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string href) { }
        public static Stormpath.SDK.Directory.IDirectory GetDirectory(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string href) { }
        public static Stormpath.SDK.Group.IGroup GetGroup(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string href) { }
        public static Stormpath.SDK.Organization.IOrganization GetOrganization(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string href) { }
        public static Stormpath.SDK.Account.IAccount VerifyAccountEmail(this Stormpath.SDK.Tenant.ITenantActions tenantActions, string token) { }
    }
}
namespace Stormpath.SDK.Tenant
{
    
    public interface IHasTenant
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Tenant.ITenant> GetTenantAsync(System.Threading.CancellationToken cancellationToken = null);
    }
    public interface ITenant : Stormpath.SDK.Resource.IAuditable, Stormpath.SDK.Resource.IExtendable, Stormpath.SDK.Resource.IResource, Stormpath.SDK.Tenant.ITenantActions
    {
        string Key { get; }
        string Name { get; }
    }
    public interface ITenantActions
    {
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> CreateApplicationAsync(Stormpath.SDK.Application.IApplication application, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> CreateApplicationAsync(Stormpath.SDK.Application.IApplication application, System.Action<Stormpath.SDK.Application.ApplicationCreationOptionsBuilder> creationOptionsAction, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> CreateApplicationAsync(Stormpath.SDK.Application.IApplication application, Stormpath.SDK.Application.IApplicationCreationOptions creationOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> CreateApplicationAsync(string name, bool createDirectory, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> CreateDirectoryAsync(Stormpath.SDK.Directory.IDirectory directory, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> CreateDirectoryAsync(Stormpath.SDK.Directory.IDirectory directory, System.Action<Stormpath.SDK.Directory.DirectoryCreationOptionsBuilder> creationOptionsAction, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> CreateDirectoryAsync(Stormpath.SDK.Directory.IDirectory directory, Stormpath.SDK.Directory.IDirectoryCreationOptions creationOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> CreateDirectoryAsync(string name, string description, Stormpath.SDK.Directory.DirectoryStatus status, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Organization.IOrganization> CreateOrganizationAsync(Stormpath.SDK.Organization.IOrganization organization, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Organization.IOrganization> CreateOrganizationAsync(Stormpath.SDK.Organization.IOrganization organization, System.Action<Stormpath.SDK.Organization.OrganizationCreationOptionsBuilder> creationOptionsAction, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Organization.IOrganization> CreateOrganizationAsync(Stormpath.SDK.Organization.IOrganization organization, Stormpath.SDK.Organization.IOrganizationCreationOptions creationOptions, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Organization.IOrganization> CreateOrganizationAsync(string name, string description, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> GetAccountAsync(string href, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Account.IAccount> GetAccounts();
        System.Threading.Tasks.Task<Stormpath.SDK.Application.IApplication> GetApplicationAsync(string href, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Application.IApplication> GetApplications();
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Directory.IDirectory> GetDirectories();
        System.Threading.Tasks.Task<Stormpath.SDK.Directory.IDirectory> GetDirectoryAsync(string href, System.Threading.CancellationToken cancellationToken = null);
        System.Threading.Tasks.Task<Stormpath.SDK.Group.IGroup> GetGroupAsync(string href, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Group.IGroup> GetGroups();
        System.Threading.Tasks.Task<Stormpath.SDK.Organization.IOrganization> GetOrganizationAsync(string href, System.Threading.CancellationToken cancellationToken = null);
        Stormpath.SDK.Linq.IAsyncQueryable<Stormpath.SDK.Organization.IOrganization> GetOrganizations();
        System.Threading.Tasks.Task<Stormpath.SDK.Account.IAccount> VerifyAccountEmailAsync(string token, System.Threading.CancellationToken cancellationToken = null);
    }
}