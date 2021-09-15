using Microsoft.Toolkit.Mvvm.ComponentModel;
using MicrosoftRewardsBot.Contracts.Services;
using MicrosoftRewardsBot.Contracts.ViewModels;
using MSRewardsBOT.Core.Contracts.Services;
using MSRewardsBOT.Core.Models;
using System.Collections.ObjectModel;

namespace MicrosoftRewardsBot.ViewModels
{
    public class MainViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private IAccountDataService _accountDataService;
        private ObservableCollection<Account> accountList = new ObservableCollection<Account>();

        public ObservableCollection<Account> AccountList { get => accountList; set => SetProperty(ref accountList, value); }

        public MainViewModel(INavigationService navigationAware, IAccountDataService accountDataService)
        {
            _navigationService = navigationAware;
            _accountDataService = accountDataService;
        }

        public async void OnNavigatedToAsync(object parameter)
        {
            accountList.Clear();


            var data = await _accountDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                AccountList.Add(item);
            }
        }

        public void OnNavigatedFrom()
        {
            return;
        }


    }
}
