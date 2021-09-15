using System.Threading.Tasks;

namespace MicrosoftRewardsBot.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle();

        Task HandleAsync();
    }
}
