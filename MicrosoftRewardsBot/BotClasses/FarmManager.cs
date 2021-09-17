using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MSRewardsBOT.Core.Contracts.Services;
using MSRewardsBOT.Core.Models;
using MSRewardsBOT.Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicrosoftRewardsBot.BotClasses
{

    public class FarmManager : ObservableObject
    {
        // Private Fields
        private bool _Error;
        private Tor _TorInstance;
        private Account _Account;
        private bool _IsPCRunning;
        private bool _IsMobileRuning;
        private int _PCProgress;
        private int _MobileProgress;
        private Browser _BrowserInstance;
        private int _ProxyPort = 9050;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }
        public Tor TorInstance { get => _TorInstance; set => SetProperty(ref _TorInstance, value); }
        private IAccountDataService AccountDataService = new AccountDataService();



        // Commands
        private ICommand _StartAccount;
        public ICommand StartAccount => _StartAccount ??= new RelayCommand(StartAllCommandcmd);
        private ICommand _StartDesktop;
        public ICommand StartDesktop => _StartDesktop ??= new RelayCommand(StartDesktopCommand);
        private ICommand _StartMobile;
        public ICommand SelectedFarm => _StartMobile ??= new RelayCommand(StartMobileCommand);





        // Public Fields

        public bool Error { get => _Error; set => SetProperty(ref _Error, value); }
        public bool IsMobileRunning { get => _IsMobileRuning; set => SetProperty(ref _IsMobileRuning, value); }
        public bool IsPCRunning { get => _IsPCRunning; set => SetProperty(ref _IsPCRunning, value); }
        public int DesktopProgress { get => _PCProgress; set => SetProperty(ref _PCProgress, value); }
        public Browser BrowserInstance { get => _BrowserInstance; set => SetProperty(ref _BrowserInstance, value); }
        public int MobileProgress { get => _MobileProgress; set => SetProperty(ref _MobileProgress, value); }
        public int ProxyPort { get => _ProxyPort; set => _ProxyPort = value; }


        public FarmManager(Account account)
        {
            Account = account;
            TorInstance = new Tor(Account.ID, ProxyPort);

        }


        private async Task AccounsearchAsync()
        {
            TorInstance = new Tor(_Account.ID);
            if (!await _TorInstance.ConnectAsync())
            {
                var task = _TorInstance.ConnectAsync();
                if (await Task.WhenAny(task, Task.Delay(3000)) != task || !task.Result)
                {
                    Account.IsError = true;
                    return;
                }

            }
            await MobilesearchAsync();
            BrowserInstance.Waiter();
            await DesktopsearcAsync();
            AccountDataService.UpdateAccount(Account);
        }

        private async Task DesktopsearcAsync()
        {
            IsPCRunning = true;
            Account.IsError = false;
            DesktopProgress = 5;
            if (!TorInstance.IsConnectionValid)
            {
                if (!await _TorInstance.ConnectAsync())
                {
                    Account.IsError = true;
                    IsPCRunning = false;

                    DesktopProgress = 0;
                    return;
                }

            }
            BrowserInstance = new Browser(Account);
            BrowserInstance.Gettrending();
            DesktopProgress = 15;
            if (! BrowserInstance.Browsersetup(Browser.BrowserMode.Desktop))
            {
                Account.IsError = true;
                IsPCRunning = false;
                DesktopProgress = 0;
                BrowserInstance.BrowserExit();
                return;
            }
            DesktopProgress = 25;
            if (! BrowserInstance.Login())
            {
                Account.IsError = true;
                IsPCRunning = false;

                BrowserInstance.BrowserExit();
                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 30;
            if (! BrowserInstance.SearchDesktop())
            {
                Account.IsError = true;
                IsPCRunning = false;

                BrowserInstance.BrowserExit();
                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 60;

            if (!BrowserInstance.Dailyoffers())
            {
                Account.IsError = true;
                IsPCRunning = false;

                BrowserInstance.BrowserExit();
                DesktopProgress = 0;
                return;
            }
            DesktopProgress = 90;
            AccountDataService.UpdateAccount(Account);
            if (!BrowserInstance.BrowserExit())
            {
                Account.IsError = true;
                IsPCRunning = false;

                BrowserInstance.BrowserExit();
                DesktopProgress = 0;
                return;
            }
            CleanChromeProfile(Browser.BrowserMode.Desktop, Account.ID);
            DesktopProgress = 100;
            IsPCRunning = false;
            Account.LastRun = DateTime.Today;
        }

        private async Task MobilesearchAsync()
        {
            Account.IsError = false;
            if (!TorInstance.IsConnectionValid)
            {
                if (!await _TorInstance.ConnectAsync())
                {
                    Account.IsError = true;
                    IsMobileRunning = false;
                    MobileProgress = 0;
                    _TorInstance.ExitTor();
                    return;
                }

            }

            BrowserInstance = new Browser(Account);
            BrowserInstance.Gettrending();
            if (!BrowserInstance.Browsersetup(Browser.BrowserMode.Mobile))
            {
                Account.IsError = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                BrowserInstance.BrowserExit();
                return;
            }
            IsMobileRunning = true;
            MobileProgress = 20;
            if (!BrowserInstance.Login())
            {
                Account.IsError = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                BrowserInstance.BrowserExit();
                return;
            }
            MobileProgress = 40;
            if (! _BrowserInstance.SearchMobileAsync())
            {
                Account.IsError = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                BrowserInstance.BrowserExit();
                return;
            }
            MobileProgress = 84;
            AccountDataService.UpdateAccount(Account);
            if (! BrowserInstance.BrowserExit())
            {
                Account.IsError = true;
                IsMobileRunning = false;
                MobileProgress = 0;
                BrowserInstance.BrowserExit();
                return;
            }
            MobileProgress = 100;
            IsMobileRunning = false;
            Account.LastRun = DateTime.Today;
            CleanChromeProfile(Browser.BrowserMode.Mobile, Account.ID);

        }

        public async Task StartAllCommand()
        {

            await AccounsearchAsync();


        }
        public async void StartAllCommandcmd()
        {
            try
            {
                await Task.Run(async () =>
                {
                    await AccounsearchAsync();
                });
            }
            catch (Exception) { }

        }

        private async void StartDesktopCommand()
        {
            try
            {
                await Task.Run(async () =>
                {
                    await DesktopsearcAsync();
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

        
        private void CleanChromeProfile(Browser.BrowserMode bmode, int iD)
        {
            string address;
            if (bmode == Browser.BrowserMode.Desktop) { address = Properties.Resources.ChromeProfileLocation + "de" + Account.ID + "\\"; }
            else
            {
                address = Properties.Resources.ChromeProfileLocation + "mo" + Account.ID + "\\";
            }
            var RootDirectories = System.IO.Directory.GetDirectories(Properties.Resources.ChromeProfileLocation);


            foreach (var FirstChild in Directory.GetDirectories(address))
            {
                if (FirstChild.Contains("Default"))
                {
                    foreach (var LastChild in Directory.GetDirectories(FirstChild))
                    {
                        Directory.Delete(LastChild, true);
                    }

                }
                else
                {
                    Directory.Delete(FirstChild, true);
                }
            }

        }



    }
}
