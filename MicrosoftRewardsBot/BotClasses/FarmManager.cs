using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MSRewardsBOT.Core.Contracts.Services;
using MSRewardsBOT.Core.Models;
using MSRewardsBOT.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicrosoftRewardsBot.BotClasses
{

    public class FarmManager : ObservableObject
    {
        // Private Fields
        private bool error;
        private Tor _TorInstance;
        private Account _Account;
        private bool _IsPCRunning;
        private bool _IsMobileRuning;
        private int _PCProgress;
        private int _MobileProgress;
        private Browser _BrowserInstance;
        private int _ProxyPort=9050;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }
        public Tor TorInstance { get => _TorInstance; set => SetProperty(ref _TorInstance, value); }
        private IAccountDataService AccountDataService = new AccountDataService();



        // Commands
        private ICommand _StartAccount;
        public ICommand StartAccount => _StartAccount ??= new RelayCommand(StartAllCommand);
        private ICommand _StartDesktop;
        public ICommand StartDesktop => _StartDesktop ??= new RelayCommand(StartDesktopCommand);
        private ICommand _StartMobile;
        public ICommand StartMobile => _StartMobile ??= new RelayCommand(StartMobileCommand);





        // Public Fields

        public bool Error { get => error; set => SetProperty(ref error, value); }
        public bool IsMobileRunning { get => _IsMobileRuning; set => SetProperty(ref _IsMobileRuning, value); }
        public bool IsPCRunning { get => _IsPCRunning; set => SetProperty(ref _IsPCRunning, value); }
        public int DesktopProgress { get => _PCProgress; set => SetProperty(ref _PCProgress, value); }
        public Browser BrowserInstance { get => _BrowserInstance; set => SetProperty(ref _BrowserInstance, value); }
        public int MobileProgress { get => _MobileProgress; set => SetProperty(ref _MobileProgress, value); }
        public int ProxyPort { get => _ProxyPort; set => _ProxyPort = value; }

        private async void AccounsearchAsync()
        {
            TorInstance = new Tor(_Account.ID);
            if (!await _TorInstance.ConnectAsync())
            {
                var task = _TorInstance.ConnectAsync();
                if (await Task.WhenAny(task, Task.Delay(3000)) != task || !task.Result)
                {
                    error = true;
                    return;
                }

            }
            MobilesearchAsync();
            await BrowserInstance.Waiter();
            DesktopsearcAsync();
            AccountDataService.UpdateAccount(Account);
        }

        private async void DesktopsearcAsync()
        {
            IsPCRunning = true;
            error = false;
            DesktopProgress = 5;
            if (!TorInstance.IsConnectionValid)
            {
                if (!await _TorInstance.ConnectAsync())
                {
                    error = true;
                    IsPCRunning = false;

                    DesktopProgress = 0;
                    return;
                }

            }
            BrowserInstance = new Browser(Account);
            BrowserInstance.Gettrending();
            DesktopProgress = 15;
            if (!await BrowserInstance.Browsersetup(Browser.BrowserMode.Desktop))
            {
                error = true;
                IsPCRunning = false;
                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 25;
            if (!await BrowserInstance.Login())
            {
                error = true;
                IsPCRunning = false;

                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 30;
            if (!await BrowserInstance.DesktopSearch())
            {
                error = true;
                IsPCRunning = false;

                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 60;

            if (!await BrowserInstance.Dailyoffers())
            {
                error = true;
                IsPCRunning = false;

                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 90;
            if (!await _BrowserInstance.BrowserExit())
            {
                error = true;
                IsPCRunning = false;

                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 100;
            IsPCRunning = false;
            AccountDataService.UpdateAccount(Account);
        }

        private async void MobilesearchAsync()
        {
            error = false;
            if (!TorInstance.IsConnectionValid)
            {
                if (!await _TorInstance.ConnectAsync())
                {
                    error = true;
                    IsMobileRunning = false;
                    MobileProgress = 0;
                    return;
                }

            }

            BrowserInstance = new Browser(Account);
            BrowserInstance.Gettrending();
            if (!await BrowserInstance.Browsersetup(Browser.BrowserMode.Mobile))
            {
                error = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                return;
            }
            IsMobileRunning = true;
            MobileProgress = 20;
            if (!await BrowserInstance.Login())
            {
                error = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                return;
            }
            MobileProgress = 40;
            if (!await _BrowserInstance.MobileSearch())
            {
                error = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                return;
            }
            MobileProgress = 84;
            if (!await _BrowserInstance.BrowserExit())
            {
                error = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                return;
            }
            MobileProgress = 100;
            IsMobileRunning = false;
            AccountDataService.UpdateAccount(Account);
        }

        private async void StartAllCommand()
        {
            try
            {
                await Task.Run(() =>
                {
                    AccounsearchAsync();
                });
            }
            catch (Exception) { }



        }

        private async void StartDesktopCommand()
        {
            try
            {
                await Task.Run(() =>
                {
                    DesktopsearcAsync();
                });
            }
            catch (Exception) { }



        }
        private async void StartMobileCommand()
        {
            try
            {
                await Task.Run(() =>
                {
                    MobilesearchAsync();
                });
            }
            catch (Exception) { }



        }

        public FarmManager(Account account)
        {
            Account = account;
            TorInstance = new Tor(Account.ID,ProxyPort);

        }


    }
}
