using Microsoft.Toolkit.Mvvm.ComponentModel;
using MicrosoftRewardsBot.BotClasses;
using MicrosoftRewardsBot.Contracts.Services;
using MicrosoftRewardsBot.Contracts.ViewModels;
using MicrosoftRewardsBot.Models;
using MSRewardsBOT.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MicrosoftRewardsBot.ViewModels
{
    public class ExecutePageVM : ObservableObject, INavigationAware
    {

        // Private Fileds
        ObservableCollection<FarmManager> _farms = new ObservableCollection<FarmManager>();
        private readonly IAccountDataService _accountDataService;
        private object selectedFarm;
        private AppTheme _theme;
        private readonly IThemeSelectorService _themeSelectorService;


        // Public Props
        public AppTheme Theme
        {
            get { return _theme; }
            set { SetProperty(ref _theme, value); }
        }
        public ObservableCollection<FarmManager> Farms { get => _farms; set => SetProperty(ref _farms, value); }
        public object SelectedFarm { get => selectedFarm; set => SetProperty(ref selectedFarm, value); }
        private readonly INavigationService _navigationService;
        

        public ExecutePageVM(INavigationService navigationService, IAccountDataService accountDataService, IThemeSelectorService themeSelectorService)
        {
            _navigationService = navigationService;
            _accountDataService = accountDataService;
            _themeSelectorService = themeSelectorService;
        }

        public void OnNavigatedFrom()
        {
            return;
        }

        public void OnNavigatedToAsync(object parameter)
        {
            Theme = _themeSelectorService.GetCurrentTheme();
            foreach (var account in _accountDataService.GetGridDataAsync().Result)
            {

                Farms.Add(new FarmManager(account));
            }
            SelectedFarm = Farms[0];
            
        }
    }
}
