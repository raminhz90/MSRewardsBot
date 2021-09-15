using System;

using MicrosoftRewardsBot.Models;

namespace MicrosoftRewardsBot.Contracts.Services
{
    public interface IThemeSelectorService
    {
        void InitializeTheme();

        void SetTheme(AppTheme theme);

        AppTheme GetCurrentTheme();
    }
}
