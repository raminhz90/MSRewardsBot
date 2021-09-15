using Microsoft.Toolkit.Mvvm.ComponentModel;
using MicrosoftRewardsBot.Contracts.Services;
using MicrosoftRewardsBot.Contracts.ViewModels;
using MSRewardsBOT.Core.Contracts.Services;
using MSRewardsBOT.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MicrosoftRewardsBot.ViewModels
{
    public class UserVM : ObservableObject, INavigationAware
    {
        private ObservableCollection<Account> _AccountList = new ObservableCollection<Account>();
        private readonly INavigationService _navigationService;
        private IAccountDataService _accountDataService;


        public UserVM(INavigationService navigationAware, IAccountDataService accountDataService)
        {
            _navigationService = navigationAware;
            _accountDataService = accountDataService;

        }

        public ObservableCollection<Account> AccountList { get => _AccountList; set => _AccountList = value; }

        public void OnNavigatedFrom()
        {
            _accountDataService.SaveDatabase(AccountList);
        }

        public async void OnNavigatedToAsync(object parameter)
        {
            AccountList.Clear();


            var data = await _accountDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                AccountList.Add(item);
            }
        }
    }
}
