using System;
using System.Windows;

using ControlzEx.Theming;

using MahApps.Metro.Theming;

using MicrosoftRewardsBot.Contracts.Services;
using MicrosoftRewardsBot.Models;

namespace MicrosoftRewardsBot.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private const string HcDarkTheme = "pack://application:,,,/Styles/Themes/HC.Dark.Blue.xaml";
        private const string HcLightTheme = "pack://application:,,,/Styles/Themes/HC.Light.Blue.xaml";

        public ThemeSelectorService()
        {
        }

        public void InitializeTheme()
        {
            // TODO WTS: Mahapps.Metro supports syncronization with high contrast but you have to provide custom high contrast themes
            // We've added basic high contrast dictionaries for Dark and Light themes
            // Please complete these themes following the docs on https://mahapps.com/docs/themes/thememanager#creating-custom-themes
            ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcDarkTheme), MahAppsLibraryThemeProvider.DefaultInstance));
            ThemeManager.Current.AddLibraryTheme(new LibraryTheme(new Uri(HcLightTheme), MahAppsLibraryThemeProvider.DefaultInstance));

            var theme = GetCurrentTheme();
            SetTheme(theme);
            if (theme == AppTheme.Light)
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;

            }
            else
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
            }
        }

        public void SetTheme(AppTheme theme)
        {
            if (theme == AppTheme.Default)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
                ThemeManager.Current.SyncTheme();
                ThemeManager.Current.ChangeTheme(Application.Current, $"Dark.Blue", SystemParameters.HighContrast);

                var k = ThemeManager.Current.BaseColors;
                
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
            }
            else
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
                ThemeManager.Current.SyncTheme();
                ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.Blue", SystemParameters.HighContrast);
            }
            if (theme == AppTheme.Light)
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;

            }
            else
            {
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
            }

            App.Current.Properties["Theme"] = theme.ToString();
        }

        public AppTheme GetCurrentTheme()
        {
            if (App.Current.Properties.Contains("Theme"))
            {
                var themeName = App.Current.Properties["Theme"].ToString();
                Enum.TryParse(themeName, out AppTheme theme);
                return theme;
            }

            return AppTheme.Dark;
        }
    }
}
