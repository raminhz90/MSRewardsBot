using System.Windows.Controls;

namespace MicrosoftRewardsBot.Contracts.Views
{
    public interface IShellWindow
    {
        Frame GetNavigationFrame();

        void ShowWindow();

        void CloseWindow();
    }
}
