using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;

using MicrosoftRewardsBot.Contracts.Views;
using MicrosoftRewardsBot.ViewModels;

namespace MicrosoftRewardsBot.Views
{
    public partial class ShellWindow : Window, IShellWindow
    {
        public ShellWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public Frame GetNavigationFrame()
            => shellFrame;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();
    }
}
