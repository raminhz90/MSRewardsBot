using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MicrosoftRewardsBot.CustomControls
{
    /// <summary>
    /// Interaction logic for AccountProgressView.xaml
    /// </summary>
    public partial class ProgressCircle : UserControl
    {

        public int ProgressValue
        {
            get => (int)GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }
        public ICommand BtnCommand
        {
            get => (ICommand)GetValue(BtnCommandProperty);
            set => SetValue(BtnCommandProperty, value);
        }
        public string BtnText
        {
            get => (string)GetValue(BtnTextProperty);
            set => SetValue(BtnTextProperty, value);
        }
        public bool IsStarted
        {
            get => (bool)GetValue(IsStartedProperty);
            set => SetValue(IsStartedProperty, value);
        }

        public Brush MiddleCircleColor
        {
            get => (Brush)GetValue(MiddleCircleColorProperty);
            set => SetValue(MiddleCircleColorProperty, value);
        }
        public double MiddleCircleAngle
        {
            get => (double)GetValue(MiddleCircleAngleProperty);
            set => SetValue(MiddleCircleAngleProperty, value);
        }
        public static readonly DependencyProperty MiddleCircleColorProperty = DependencyProperty.Register("MiddleCircleColor",
            typeof(Brush), typeof(ProgressCircle), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty MiddleCircleAngleProperty = DependencyProperty.Register("MiddleCircleAngle",
            typeof(double), typeof(ProgressCircle), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty BtnTextProperty = DependencyProperty.Register("BtnText",
            typeof(string), typeof(ProgressCircle), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IsStartedProperty = DependencyProperty.Register("IsStarted",
            typeof(bool), typeof(ProgressCircle), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue",
            typeof(int), typeof(ProgressCircle), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty BtnCommandProperty = DependencyProperty.Register("BtnCommand",
            typeof(ICommand), typeof(ProgressCircle), new PropertyMetadata(default(ICommand)));


        public ProgressCircle()
        {
            InitializeComponent();

        }
    }
}
