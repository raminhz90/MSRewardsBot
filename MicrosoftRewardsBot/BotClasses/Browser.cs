using Microsoft.Toolkit.Mvvm.ComponentModel;
using MSRewardsBOT.Core.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MicrosoftRewardsBot.BotClasses

{
    public class Browser : ObservableObject
    {
        public enum BrowserMode
        {
            Mobile,
            Desktop

        }
        private readonly int MaxPcSearch = int.Parse(Properties.Resources.MaxPCSearch);
        private readonly int MaxMobileSearch = int.Parse(Properties.Resources.MaxMobileSearch);
        private static string BING_SEARCH_URL = "https://www.bing.com/search";
        private static string DASHBOARD_URL = "https://account.microsoft.com/rewards/dashboard";

        private ChromeDriverService driverService;
        private Account _account;
        private int _PcSearch;
        private int _MobileSearch;
        private int _OffersAvailable;
        private int _MaxDesktopPoints = 200;
        private int _MaxMobilePoints = 200;
        private int _MaxEdgePoints = 200;
        private int _EdgePoints;

        private int _proxyport;
        private List<string> searchqueries = new List<string>();
        private ChromeDriver chromeDriver;

        public Browser(Account account, int proxyport = 9050)
        {
            _account = account;
            Proxyport = proxyport;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);
        }
        public async Task Waiter(int mili = 100)
        {
            Thread.Sleep(mili);
        }

        public async Task StartPointGathering(Account account, int proxy)
        {

            Account = account;
            Proxyport = proxy;
            Gettrending();
            Account.Progress = 10;
            Browsersetup(BrowserMode.Mobile);
            if (await Login())
            {

            }
            else
            {
                return;
            }
            await MobileSearch();

            OnAppExit(1, new EventArgs());
            Thread.Sleep(100);
            account.Progress = 50;
            await Browsersetup(BrowserMode.Desktop);
            if (!await Login())
            {
            }
            else
            {
                return;
            }
            await DesktopSearch();
            await Dailyoffers();
            OnAppExit(1, new EventArgs());
        }

        public Account Account { get => _account; set => SetProperty(ref _account, value); }


        public int Proxyport { get => _proxyport; set => _proxyport = value; }
        public int PcSearch { get => _PcSearch; set => _PcSearch = value; }
        public int MobileSearchV { get => _MobileSearch; set => SetProperty(ref _MobileSearch, value); }
        public int EdgePoints { get => _EdgePoints; set => _EdgePoints = value; }
        public int MaxDesktopPoints { get => MaxDesktopPoints1; set => MaxDesktopPoints1 = value; }
        public int MaxMobilePoints { get => _MaxMobilePoints; set => _MaxMobilePoints = value; }
        public int MaxDesktopPoints1 { get => _MaxDesktopPoints; set => _MaxDesktopPoints = value; }


        public async Task<bool> Browsersetup(BrowserMode mode)
        {
            driverService = ChromeDriverService.CreateDefaultService(@".\Browser\");
            driverService.HideCommandPromptWindow = true;
            ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.BinaryLocation = @".\browser\chrome.exe";
            //chromeOptions.AddExtension(@".\ext.crx");
            //chromeOptions.AddExtension(@"d:\rewards.crx");
            chromeOptions.AddArgument("--disable-webgl");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument(@"--proxy-server=socks5://127.0.0.1:" + Proxyport);

            chromeOptions.AddArgument("--disable-notifications");
            List<string> experimentalFlags = new List<string>();
            experimentalFlags.Add("same-site-by-default-cookies@2");
            experimentalFlags.Add("cookies-without-same-site-must-be-secure@2");
            experimentalFlags.Add("profile.default_content_setting_values.cookies@1");
            //experimentalFlags.Add("network.cookie.cookieBehavior@1");
            //experimentalFlags.Add("profile.block_third_party_cookies@0");
            //chromeOptions.AddUserProfilePreference("browser.enabled_labs_experiments", experimentalFlags);
            chromeOptions.AddUserProfilePreference("deleteDataPostSession", false);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.cookies", 1);
            chromeOptions.AddUserProfilePreference("profile.cookie_controls_mode", 0);
            //chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
            //driverService.Start();
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            if (mode == BrowserMode.Desktop)
            {
                chromeOptions.AddArgument("user-data-dir=" + Properties.Resources.ChromeProfileLocation + "de" + Account.ID + "\\");
                chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36 Edg/93.0.961.44");
            }
            else
            {
                chromeOptions.EnableMobileEmulation("iPhone X");
                chromeOptions.AddArgument("user-data-dir=" + Properties.Resources.ChromeProfileLocation + "mo" + Account.ID + "\\");
                chromeOptions.AddArgument("user-agent=Mozilla / 5.0(Linux; Android 10; Redmi Note 9 Pro) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 77.0.3865.116 Mobile Safari/ 537.36 EdgA / 46.06.4.5161");
            }
            chromeDriver = new ChromeDriver(driverService, chromeOptions);
            chromeDriver.Manage().Cookies
            return true;
        }


        //TODO: Consider Other Scenarios And Errors
        public async Task<bool> Login()
        {


            chromeDriver.Navigate().GoToUrl(@"https://login.live.com/");
            //driver.Navigate().GoToUrl(@"https://api.ipify.org");
            Thread.Sleep(1000);
            if (chromeDriver.Url == @"https://login.live.com/")
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                try
                {

                    _ = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("loginfmt")));
                    chromeDriver.FindElement(By.Name("loginfmt")).SendKeys(Account.UserName);
                    chromeDriver.FindElement(By.Name("loginfmt")).SendKeys(Keys.Return);
                }
                catch (Exception e)
                {
                    return false;
                }
                Thread.Sleep(1000);
                try
                {
                    if (chromeDriver.FindElements(By.Name("passwd")).Count != 0)
                    {
                        _ = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Name("passwd")));
                        chromeDriver.FindElement(By.Name("passwd")).SendKeys(Account.Password);
                        Thread.Sleep(100);
                        //chromeDriver.FindElement(By.Id("idChkBx_PWD_KMSI0Pwd")).Click();
                        chromeDriver.FindElement(By.Name("passwd")).SendKeys(Keys.Return);
                        chromeDriver.FindElement(By.Id("idSIButton9")).Click();
                    }
                    else
                    {
                        chromeDriver.FindElement(By.XPath("//*[@id=\"idA_PWD_SwitchToPassword\"]")).Click();
                        _ = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("passwd")));
                        chromeDriver.FindElement(By.Name("passwd")).SendKeys(Account.Password);
                        Thread.Sleep(100);
                        chromeDriver.FindElement(By.Id("idChkBx_PWD_KMSI0Pwd")).Click();
                        chromeDriver.FindElement(By.Name("passwd")).SendKeys(Keys.Return);
                        chromeDriver.FindElement(By.Id("idSIButton9")).Click();
                    }

                }
                catch (Exception e)
                {



                    return false;
                }
                try
                {
                    _ = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("uhfLogo")));
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {

                return true;
            }
            return true;
        }

        public async Task<bool> DesktopSearch()
        {
            if (!await Login())
            {

                OnAppExit(1, new EventArgs());
                return false;
            }
            try
            {
                chromeDriver.Navigate().GoToUrl(DASHBOARD_URL);
                chromeDriver.FindElement(By.Id("error-title")).Text.Contains("not available");


                return false;
            }
            catch (Exception) { }
            Checkpoints();

            Random rnd = new Random();

            chromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            try
            {

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("id_l")));
                Thread.Sleep(500);

                while (chromeDriver.FindElement(By.Id("id_l")).Text == "Sign in")
                {
                    chromeDriver.FindElement(By.Id("id_l")).Click();
                    Thread.Sleep(1500);
                }
            }
            catch (Exception e) { }

            while (PcSearch <= MaxPcSearch && Account.PCPoints <= MaxDesktopPoints)
            {
                Thread.Sleep(1000);
                try
                {
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("sb_form_q")));
                    chromeDriver.FindElement(By.Id("sb_form_q")).Clear();
                    chromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(searchqueries[rnd.Next(searchqueries.Count)]);
                    Thread.Sleep(1000);
                    chromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(Keys.Return);
                }
                catch (Exception e)
                {
                    return false;
                }
                Thread.Sleep(rnd.Next(1000, 4000));
                if (PcSearch % 10 == 0)
                {
                    if (Account.Progress < 46)
                    {
                        Account.Progress += 5;
                    }
                    Checkpoints();

                }
                PcSearch += 1;
            }


            return true;
        }

        public void Gettrending()
        {

            List<string> dates = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                DateTime date = DateTime.Today - TimeSpan.FromDays(i);
                dates.Add(date.ToString("yyyyMMdd"));
            }
            foreach (var item in dates)
            {

                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString("https://trends.google.com/trends/api/dailytrends?hl=en-US&ed=" + item + "&geo=US&ns=15");
                    string result = string.Join(string.Empty, json.Skip(5));
                    SearchTrends searchtrends = new SearchTrends();
                    searchqueries.AddRange(searchtrends.gettrends(result));

                }
            }

        }

        public async Task<bool> MobileSearch()
        {



            if (!await Login())
            {

                return false;
            }

            try
            {
                chromeDriver.Navigate().GoToUrl(DASHBOARD_URL);
                chromeDriver.FindElement(By.Id("error-title")).Text.Contains("not available");

                return false;
            }
            catch (Exception)
            {
            }
            try
            {
                chromeDriver.Navigate().GoToUrl("https://www.bing.com/rewards/checkuser?ru=/search?q=popular+now+on+bing%26filters=segment%3a%22popularnow.carousel%22%26form=ml10ns%26crea=ml10ns&rnoreward=1");
                Thread.Sleep(500);
                chromeDriver.FindElementById("WLSignin").Click();
            }
            catch (Exception e)
            {

            }

            try
            {
                Checkpointsmobile();

            }
            catch (Exception e)
            {
            }
            try
            {
                Random rnd = new Random();

                chromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

                Thread.Sleep(500);




                while (MobileSearchV <= MaxMobileSearch && Account.MobilePoints <= MaxMobilePoints)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("sb_form_q")));
                        chromeDriver.FindElement(By.Id("sb_form_q")).Clear();
                        chromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(searchqueries[rnd.Next(searchqueries.Count)]);
                        Thread.Sleep(300);
                        chromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(Keys.Return);
                        Thread.Sleep(rnd.Next(1000, 4000));
                        if (MobileSearchV % 10 == 0)
                        {
                            Checkpointsmobile();

                        }
                    }
                    catch (Exception e)
                    {

                        return false;
                    }


                    MobileSearchV += 1;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        private void go_to_main()
        {
            try
            {
                for (int i = 1; i <= chromeDriver.WindowHandles.Count; i++)
                {
                    chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles[i]);
                    chromeDriver.Close();
                }

            }
            catch (Exception)
            {
            }
            chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles[0]);
        }
        private void Checkpoints()
        {
            try
            {
                chromeDriver.Navigate().GoToUrl("https://account.microsoft.com/rewards/pointsbreakdown");
                try
                {
                    chromeDriver.FindElementById("raf-signin-link-id").Click();
                    chromeDriver.Navigate().GoToUrl("https://account.microsoft.com/rewards/pointsbreakdown");
                }
                catch (Exception) { }

                var k = chromeDriver.FindElements(By.ClassName("pointsBreakdownCard"));
                string[] pcsearch = new string[3];
                string[] mobilesearch = new string[3];
                string[] edgesearch = new string[3];
                foreach (var item in k)
                {
                    if (item.Text.Contains("PC"))
                    {
                        pcsearch = Regex.Split(item.Text, @"\D+");
                    }
                    if (item.Text.Contains("Mobile"))
                    {
                        mobilesearch = Regex.Split(item.Text, @"\D+");
                    }
                    if (item.Text.Contains("Edge"))
                    {
                        edgesearch = Regex.Split(item.Text, @"\D+");
                    }
                }
                int temp;
                int.TryParse(pcsearch[1], out temp);
                Account.PCPoints = temp;
                int.TryParse(pcsearch[2], out temp);
                MaxDesktopPoints = temp;
                int.TryParse(mobilesearch[1], out temp);
                Account.MobilePoints = temp;
                int.TryParse(mobilesearch[2], out _MaxMobilePoints);

                int.TryParse(edgesearch[1], out _EdgePoints);
                int.TryParse(edgesearch[2], out _MaxEdgePoints);

                chromeDriver.Navigate().GoToUrl("https://account.microsoft.com/rewards");
                var TotalP = chromeDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/ui-view/mee-rewards-dashboard/main/mee-rewards-user-status/div/mee-banner/div/div/div/div[2]/div[1]/mee-banner-slot-2/mee-rewards-user-status-item/mee-rewards-user-status-balance/div/div/div/div/div/p[1]/mee-rewards-counter-animation/span"));
                Thread.Sleep(800);

                int.TryParse(TotalP.Text.Where(c => char.IsDigit(c)).ToArray(),
                    out temp);
                Account.TotalPoints = temp;
                chromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);

                return;
            }
            catch (Exception) { }


        }
        private void Checkpointsmobile()
        {
            //driver.Navigate().GoToUrl(POINT_TOTAL_URL);

            try
            {
                chromeDriver.Navigate().GoToUrl("https://account.microsoft.com/rewards/pointsbreakdown");

                var k = chromeDriver.FindElements(By.ClassName("pointsBreakdownCard"));
                string[] pcsearch = new string[3];
                string[] mobilesearch = new string[3];
                string[] edgesearch = new string[3];
                foreach (var item in k)
                {
                    if (item.Text.Contains("PC"))
                    {
                        pcsearch = Regex.Split(item.Text, @"\D+");
                    }
                    if (item.Text.Contains("Mobile"))
                    {
                        mobilesearch = Regex.Split(item.Text, @"\D+");
                    }
                    if (item.Text.Contains("Edge"))
                    {
                        edgesearch = Regex.Split(item.Text, @"\D+");
                    }
                }
                int temp;

                var TotalP = chromeDriver.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/ui-view/mee-rewards-dashboard/main/mee-rewards-user-status/div/mee-banner/div/div/div/div[2]/div[1]/mee-banner-slot-2/mee-rewards-user-status-item/mee-rewards-user-status-balance/div/div/div/div/div/p[1]/mee-rewards-counter-animation/span"));
                Thread.Sleep(800);

                int.TryParse(TotalP.Text.Where(c => char.IsDigit(c)).ToArray(),
                    out temp);
                Account.TotalPoints = temp;
                chromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);
                try
                {
                    int.TryParse(pcsearch[1], out temp);
                    Account.PCPoints = temp;
                    int.TryParse(pcsearch[2], out temp);
                    MaxDesktopPoints = temp;
                }
                catch (Exception)
                {

                    MaxDesktopPoints = 0;
                }

                try
                {
                    int.TryParse(mobilesearch[1], out temp);
                    Account.MobilePoints = temp;
                    int.TryParse(mobilesearch[2], out _MaxMobilePoints);
                }
                catch (Exception)
                {

                    _MaxMobilePoints = 0;
                }

                try
                {
                    int.TryParse(edgesearch[1], out _EdgePoints);
                    int.TryParse(edgesearch[2], out _MaxEdgePoints);
                }
                catch (Exception)
                {
                    _MaxEdgePoints = 0;


                }





            }
            catch (Exception e)
            {
            }
        }
        public async Task<bool> Dailyoffers()
        {

            try
            {

                chromeDriver.Navigate().GoToUrl(DASHBOARD_URL);
                try
                {
                    chromeDriver.FindElement(By.Id("error-title")).Text.Contains("not available");
                    chromeDriver.Quit();
                    return false;
                }
                catch (Exception)
                {


                }
                var offers = chromeDriver.FindElementsByXPath("//span[contains(@class, 'mee-icon-AddMedium')]/ancestor::a");
                if (offers.Count > 0)
                {



                    foreach (IWebElement link in offers)
                    {
                        Thread.Sleep(1500);
                        var ct = chromeDriver.WindowHandles[0];
                        link.Click();
                        var nt = chromeDriver.WindowHandles[1];
                        chromeDriver.SwitchTo().Window(nt);
                        Thread.Sleep(3500);
                        //driver.ExecuteScript("window.stop()");


                        try
                        {
                            chromeDriver.FindElement(By.Id("btoption0"));
                            Dailypoll();
                            continue;
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            var btn = chromeDriver.FindElement(By.Id("rqStartQuiz"));
                            btn.Click();
                            Thread.Sleep(1000);
                            try
                            {
                                chromeDriver.FindElement(By.Id("rqAnswerOptionNum0"));
                                drag_and_drop_quiz();
                                continue;
                            }
                            catch (Exception)
                            {
                            }
                            try
                            {
                                chromeDriver.FindElement(By.Id("rqAnswerOption0"));
                                lightning_quiz();
                                continue;
                            }
                            catch (Exception)
                            {
                            }

                        }
                        catch (Exception)
                        { }
                        try
                        {
                            chromeDriver.FindElementByClassName("wk_Circle");
                            click_quiz();
                        }
                        catch (Exception e)
                        {

                            explore_daily();
                        }
                    }
                }
                offers = chromeDriver.FindElementsByXPath("//span[contains(@class, 'mee-icon-AddMedium')]/ancestor::a");
                if (offers.Count > 0)
                {



                    foreach (IWebElement link in offers)
                    {
                        Thread.Sleep(1500);
                        var ct = chromeDriver.WindowHandles[0];
                        link.Click();
                        var nt = chromeDriver.WindowHandles[1];
                        chromeDriver.SwitchTo().Window(nt);
                        Thread.Sleep(3500);
                        //driver.ExecuteScript("window.stop()");


                        try
                        {
                            chromeDriver.FindElement(By.Id("btoption0"));
                            Dailypoll();
                            continue;
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            var btn = chromeDriver.FindElement(By.Id("rqStartQuiz"));
                            btn.Click();
                            Thread.Sleep(1000);
                            try
                            {
                                chromeDriver.FindElement(By.Id("rqAnswerOptionNum0"));
                                drag_and_drop_quiz();
                                continue;
                            }
                            catch (Exception)
                            {
                            }
                            try
                            {
                                chromeDriver.FindElement(By.Id("rqAnswerOption0"));
                                lightning_quiz();
                                continue;
                            }
                            catch (Exception)
                            {
                            }

                        }
                        catch (Exception)
                        { }
                        try
                        {
                            chromeDriver.FindElementByClassName("wk_Circle");
                            click_quiz();
                        }
                        catch (Exception)
                        {

                            explore_daily();
                        }
                    }
                }

                _OffersAvailable = chromeDriver.FindElementsByXPath("//span[contains(@class, 'mee-icon-AddMedium')]").Count;
                chromeDriver.Quit();
                return true;
            }
            catch (Exception e)
            {


                _OffersAvailable = 0;

                return false;
            }
        }

        private void explore_daily()
        {
            try
            {
                var html = chromeDriver.FindElementByTagName("html");
                for (int i = 0; i < 3; i++)
                {
                    html.SendKeys(Keys.End);
                    Thread.Sleep(50);
                    html.SendKeys(Keys.Home);
                    Thread.Sleep(200);
                }
                go_to_main();
            }
            catch (Exception e)
            {
            }
        }

        private void click_quiz()
        {
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                if (chromeDriver.FindElementsByCssSelector(".cico.btCloseBack").Count != 0)
                {
                    chromeDriver.FindElementsByCssSelector(".cico.btCloseBack")[0].Click();

                }
                var choices = chromeDriver.FindElementsByClassName("wk_Circle");
                if (choices.Count != 0)
                {
                    choices[rnd.Next(choices.Count)].Click();
                    Thread.Sleep(500);
                }
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.ClassName("wk_button")));
                chromeDriver.FindElementByClassName("wk_button").Click();
                Thread.Sleep(2000);
                if (chromeDriver.FindElementsByCssSelector("span[class=\"rw_icon\"]").Count != 0)
                {
                    break;
                }

            }
            go_to_main();
        }

        private void lightning_quiz()
        {
            for (int i = 0; i < 10; i++)
            {
                if (chromeDriver.FindElementsById("rqAnswerOption0").Count != 0)
                {
                    string firstpage = chromeDriver.FindElementById("rqAnswerOption0").GetAttribute("data-serpquery");
                    chromeDriver.Navigate().GoToUrl("https://www.bing.com" + firstpage);
                    Thread.Sleep(1500);
                    for (int j = 0; j < 10; j++)
                    {
                        if (chromeDriver.FindElementsById("rqAnswerOption" + j.ToString()).Count != 0)
                        {
                            chromeDriver.ExecuteScript(string.Format("document.querySelector('#rqAnswerOption{0}').click();", j));
                            Thread.Sleep(1500);
                        }
                    }
                }
                Thread.Sleep(1500);
                if (chromeDriver.FindElementsById("quizCompleteContainer").Count != 0)
                {
                    break;
                }
            }
            var quiz = chromeDriver.FindElementsByCssSelector(".cico.btCloseBack");
            if (quiz.Count != 0)
            {
                quiz[0].Click();
            }
            Thread.Sleep(1500);
            go_to_main();
        }

        private void drag_and_drop_quiz()
        {
            var rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    var drag_options = chromeDriver.FindElementsByClassName("rqOption");
                    var right_answers = chromeDriver.FindElementsByClassName("correctAnswer");
                    List<IWebElement> drag_opt = new List<IWebElement>();
                    if (right_answers.Count != 0)
                    {
                        foreach (var item in drag_options)
                        {
                            if (right_answers.Contains(item))
                            {
                                drag_opt.Add(item);
                            }
                        }
                    }
                    if (drag_opt.Count != 0)
                    {
                        IWebElement choice_a = drag_opt[rnd.Next(drag_opt.Count)];
                        drag_opt.Remove(choice_a);
                        IWebElement choice_b = drag_opt[rnd.Next(drag_opt.Count)];
                        Actions builder1 = new Actions(chromeDriver);
                        IAction dragAndDrop1 = builder1.ClickAndHold(choice_a).MoveToElement(choice_b).Release(choice_a).Build();
                        dragAndDrop1.Perform();
                    }
                }
                catch
                {
                    continue;
                }
                Thread.Sleep(500);
                if (chromeDriver.FindElementsById("quizCompleteContainer").Count != 0)
                {
                    break;
                }
            }
            Thread.Sleep(2500);
            var btn = chromeDriver.FindElementsByCssSelector(".cico.btCloseBack");
            if (btn.Count != 0)
            {
                btn[0].Click();
            }
            Thread.Sleep(1000);
            go_to_main();
        }

        private void Dailypoll()
        {
            var random = new Random();
            List<string> choices = new List<string>() { "btoption0", "btoption1" };
            chromeDriver.FindElement(By.Id(choices[random.Next(choices.Count)])).Click();
            Thread.Sleep(500);
            chromeDriver.FindElement(By.Id("rqCloseBtn")).Click();
            Thread.Sleep(500);
            go_to_main();
        }
        public async Task<bool> BrowserExit()
        {
            var tempid = driverService.ProcessId;
            chromeDriver.Quit();
            var driverProcessIds = new List<int> { driverService.ProcessId };
            var mos = new System.Management.ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={driverService.ProcessId}");
            foreach (var item in mos.Get())
            {
                var pid = Convert.ToInt32(item["ProcessID"]);
                driverProcessIds.Add(pid);
            }
            foreach (var id in driverProcessIds)
            {
                if (id != 0)
                {
                    try
                    {
                        System.Diagnostics.Process.GetProcessById(id).Kill();
                    }
                    catch (Exception) { }


                }

            }
            return true;

        }
        private void OnAppExit(object sender, EventArgs e)
        {

            if (driverService.IsRunning)
            {
                var driverProcessIds = new List<int> { driverService.ProcessId };
                var mos = new System.Management.ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={driverService.ProcessId}");
                foreach (var item in mos.Get())
                {
                    var pid = Convert.ToInt32(item["ProcessID"]);
                    driverProcessIds.Add(pid);
                }
                foreach (var id in driverProcessIds)
                {
                    if (id != 0)
                    {
                        try
                        {
                            System.Diagnostics.Process.GetProcessById(id).Kill();
                        }
                        catch (Exception) { }


                    }

                }
            }

        }
    }
}
