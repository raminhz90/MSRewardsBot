using MicrosoftRewardsBot.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MicrosoftRewardsBot.Views
{
    /// <summary>
    /// Interaction logic for AccountManage.xaml
    /// </summary>
    public partial class AccountManage : Page
    {
        public AccountManage(UserVM uservm)
        {
            InitializeComponent();
            DataContext = uservm;
        }
    }
}
