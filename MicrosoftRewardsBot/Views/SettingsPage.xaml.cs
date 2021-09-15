using System.Windows.Controls;

using MicrosoftRewardsBot.ViewModels;

namespace MicrosoftRewardsBot.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
