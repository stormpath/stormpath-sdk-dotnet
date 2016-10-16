using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    public interface IChallenge : IResource
    {
        ChallengeStatus Status { get; }

        // TODO what does this return?
        Task SubmitAsync(string code);
    }
}
