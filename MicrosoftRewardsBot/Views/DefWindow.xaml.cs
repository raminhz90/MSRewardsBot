using MicrosoftRewardsBot.Contracts.Views;
using MicrosoftRewardsBot.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MicrosoftRewardsBot.Views
{
    /// <summary>
    /// Interaction logic for DefWindow.xaml
    /// </summary>
    public partial class DefWindow : Window, IShellWindow
    {
        public DefWindow(ShellViewModel viewModel)
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
