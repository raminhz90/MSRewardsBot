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

namespace MicrosoftRewardsBot.CustomControls
{
    /// <summary>
    /// Interaction logic for AccountProgressView.xaml
    /// </summary>
    public partial class AccountProgressView : UserControl
    {

        public int ProgressValue
        {
            get { return (int)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        public string LogText
        {
            get { return (string)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        public static readonly DependencyProperty LogTextProperty = DependencyProperty.Register("LogText",
            typeof(string), typeof(AccountProgressView), new PropertyMetadata(default(string)));



        public static readonly DependencyProperty ProgressValueProperty
            = DependencyProperty.Register("ProgressValue", typeof(int), typeof(AccountProgressView),
                                          new PropertyMetadata(default(int)));
        public AccountProgressView()
        {
            InitializeComponent();

        }
    }
}
