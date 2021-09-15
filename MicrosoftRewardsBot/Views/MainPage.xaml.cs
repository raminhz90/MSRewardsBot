using System.Windows.Controls;

using MicrosoftRewardsBot.ViewModels;

namespace MicrosoftRewardsBot.Views
{
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
