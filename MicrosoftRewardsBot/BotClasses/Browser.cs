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
        private readonly string BING_SEARCH_URL = Properties.Resources.BING_SEARCH_URL;
        private static string DASHBOARD_URL = Properties.Resources.Dashboard_Url;
        private ChromeDriverService driverService;
        private int PcSearch;
        private int MobileSearch;
        private int _IncompleteOffers;
        private ChromeDriver ChromeDriver;
        private List<string> searchqueries = new List<string>();
        // -------------------------Property--------------------------------
        private Account _Account;
        public Account Account { get => _Account; set => SetProperty(ref _Account, value); }
        private int _MaxDesktopPoints = 200;
        public int MaxDesktopPoints { get => _MaxDesktopPoints; set => SetProperty(ref _MaxDesktopPoints, value); }
        private int _MaxMobilePoints = 200;
        public int MaxMobilePoints { get => _MaxMobilePoints; set => SetProperty(ref _MaxMobilePoints, value); }
        private int _MaxEdgePoints = 200;
        private int _EdgePoints;
        public int EdgePoints { get => _EdgePoints; set => SetProperty(ref _EdgePoints, value); }
        private int Proxyport;

        public Browser(Account account, int proxyport = 9050)
        {
            Account = account;
            Proxyport = proxyport;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);
        }
        public void Waiter(int Time = 100) => Thread.Sleep(Time);

        //public async Task StartPointGathering(Account account, int proxy)
        //{

        //    Account = account;
        //    Proxyport = proxy;
        //    Gettrending();
        //    Account.Progress = 10;
        //    Browsersetup(BrowserMode.Mobile);
        //    if (await Login())
        //    {

        //    }
        //    else
        //    {
        //        return;
        //    }
        //    await SearchMobileAsync();

        //    OnAppExit(1, new EventArgs());
        //    Thread.Sleep(100);
        //    account.Progress = 50;
        //    await Browsersetup(BrowserMode.Desktop);
        //    if (!await Login())
        //    {
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    await SearchDesktop();
        //    await Dailyoffers();
        //    OnAppExit(1, new EventArgs());
        //}

        public  bool Browsersetup(BrowserMode mode)
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
            ChromeDriver = new ChromeDriver(driverService, chromeOptions);
            return true;
        }

        // Daily Mobile Search
        public bool SearchMobileAsync()
        {



            

            try
            {
                ChromeDriver.Navigate().GoToUrl(DASHBOARD_URL);
                ChromeDriver.FindElement(By.XPath("//*[@id=\"rewards - error\"]")).Text.Contains("not available");

                return false;
            }
            catch (Exception) { }
            try
            {
                ChromeDriver.Navigate().GoToUrl("https://www.bing.com/rewards/checkuser?ru=/search?q=popular+now+on+bing%26filters=segment%3a%22popularnow.carousel%22%26form=ml10ns%26crea=ml10ns&rnoreward=1");
                Thread.Sleep(500);
                ChromeDriver.FindElementById("WLSignin").Click();
            }
            catch (Exception e) { }


            Checkpointsmobile();



            try
            {
                Random rnd = new Random();

                ChromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);
                WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));

                Thread.Sleep(500);




                while (MobileSearch < MaxMobileSearch && Account.MobilePoints < MaxMobilePoints)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("sb_form_q")));
                        ChromeDriver.FindElement(By.Id("sb_form_q")).Clear();
                        ChromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(searchqueries[rnd.Next(searchqueries.Count)]);
                        Thread.Sleep(300);
                        ChromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(Keys.Return);
                        Thread.Sleep(rnd.Next(1000, 4000));
                        if (MobileSearch % 10 == 0)
                        {
                            Checkpointsmobile();

                        }
                    }
                    catch (Exception e)
                    {

                        return false;
                    }


                    MobileSearch += 1;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        // Daily Desktop Searches
        public bool SearchDesktop()
        {
            
            try
            {
                ChromeDriver.Navigate().GoToUrl(DASHBOARD_URL);
                ChromeDriver.FindElement(By.XPath("//*[@id=\"rewards - error\"]")).Text.Contains("not available");


                return false;
            }
            catch (Exception) { }
            try
            {
                Checkpoints();
            }
            catch (Exception)
            {

                return false;
            }

            Random rnd = new Random();


            WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));
            try
            {
                ChromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("id_l")));
                Thread.Sleep(500);

                while (ChromeDriver.FindElement(By.Id("id_l")).Text == "Sign in")
                {
                    ChromeDriver.FindElement(By.Id("id_l")).Click();
                    Thread.Sleep(150);
                    ChromeDriver.Navigate().Refresh();
                    Thread.Sleep(1500);
                }
            }
            catch (Exception e) { return false; }

            while (PcSearch < MaxPcSearch && Account.PCPoints < MaxDesktopPoints)
            {
                Thread.Sleep(1000);
                try
                {
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("sb_form_q")));
                    ChromeDriver.FindElement(By.Id("sb_form_q")).Clear();
                    ChromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(searchqueries[rnd.Next(searchqueries.Count)]);
                    Thread.Sleep(1000);
                    ChromeDriver.FindElement(By.Id("sb_form_q")).SendKeys(Keys.Return);
                }
                catch (Exception) { }
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

        //TODO: Consider Other Scenarios And Errors
        public bool Login()
        {
            try
            {
                ChromeDriver.Navigate().GoToUrl(Properties.Resources.Login_page);
                Thread.Sleep(1000);
                if (ChromeDriver.Url == Properties.Resources.Login_page)
                {
                    WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));
                    _ = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("loginfmt")));
                    ChromeDriver.FindElement(By.Name("loginfmt")).SendKeys(Account.UserName);
                    ChromeDriver.FindElement(By.Name("loginfmt")).SendKeys(Keys.Return);
                    Thread.Sleep(1000);
                    if (ChromeDriver.FindElements(By.Name("passwd")).Count != 0)
                    {
                        _ = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Name("passwd")));
                        ChromeDriver.FindElement(By.Name("passwd")).SendKeys(Account.Password);
                        Thread.Sleep(100);
                        //chromeDriver.FindElement(By.Id("idChkBx_PWD_KMSI0Pwd")).Click();
                        ChromeDriver.FindElement(By.Name("passwd")).SendKeys(Keys.Return);
                        ChromeDriver.FindElement(By.Id("idSIButton9")).Click();
                    }
                    else
                    {
                        ChromeDriver.FindElement(By.XPath("//*[@id=\"idA_PWD_SwitchToPassword\"]")).Click();
                        _ = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name("passwd")));
                        ChromeDriver.FindElement(By.Name("passwd")).SendKeys(Account.Password);
                        Thread.Sleep(100);
                        ChromeDriver.FindElement(By.Id("idChkBx_PWD_KMSI0Pwd")).Click();
                        ChromeDriver.FindElement(By.Name("passwd")).SendKeys(Keys.Return);
                        ChromeDriver.FindElement(By.Id("idSIButton9")).Click();
                        Thread.Sleep(100);
                        _ = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("uhfLogo")));
                    }
                }


            }
            catch (System.Exception)
            {

                return false;
            }
            try
            {
                ChromeDriver.Navigate().GoToUrl(Properties.Resources.Dashboard_Url);
                ChromeDriver.FindElement(By.XPath("//*[@id=\"rewards - error\"]")).Text.Contains("not available");
                return false;


            }
            catch (Exception) { }
            return true;
        }


        // Get Trending Search topics 
        //ToDo: add fallback if google denies access
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

        // Closes other tab and returns to original tab
        private void go_to_main()
        {
            try
            {
                for (int i = 1; i <= ChromeDriver.WindowHandles.Count; i++)
                {
                    ChromeDriver.SwitchTo().Window(ChromeDriver.WindowHandles[i]);
                    ChromeDriver.Close();
                }

            }
            catch (Exception) { }
            ChromeDriver.SwitchTo().Window(ChromeDriver.WindowHandles[0]);
        }
        // Checks Point Stats in Desktop mode
        private bool Checkpoints()
        {
            try
            {
                ChromeDriver.Navigate().GoToUrl(Properties.Resources.PonisBreakdownPage);
                try
                {
                    ChromeDriver.FindElementById("raf-signin-link-id").Click();
                    ChromeDriver.Navigate().GoToUrl(Properties.Resources.PonisBreakdownPage);
                }
                catch (Exception) { }

                var k = ChromeDriver.FindElements(By.ClassName("pointsBreakdownCard"));
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

                ChromeDriver.Navigate().GoToUrl("https://account.microsoft.com/rewards");
                var TotalP = ChromeDriver.FindElement(By.XPath(Properties.Resources.TotalPointsXPath));
                Thread.Sleep(800);

                int.TryParse(TotalP.Text.Where(c => char.IsDigit(c)).ToArray(),
                    out temp);
                Account.TotalPoints = temp;
                ChromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);


            }
            catch (Exception) { return false; }

            return true;
        }
        // Checks Point Stats in Mobile mode
        private bool Checkpointsmobile()
        {
            //driver.Navigate().GoToUrl(POINT_TOTAL_URL);

            try
            {
                ChromeDriver.Navigate().GoToUrl(Properties.Resources.PonisBreakdownPage);

                var k = ChromeDriver.FindElements(By.ClassName("pointsBreakdownCard"));
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
                ChromeDriver.Navigate().GoToUrl("https://account.microsoft.com/rewards");
                var TotalP = ChromeDriver.FindElement(By.XPath(Properties.Resources.TotalPointsXPathm));
                Thread.Sleep(800);

                int.TryParse(TotalP.Text.Where(c => char.IsDigit(c)).ToArray(),
                    out temp);
                Account.TotalPoints = temp;
                ChromeDriver.Navigate().GoToUrl(BING_SEARCH_URL);
                try
                {
                    int.TryParse(pcsearch[1], out temp);
                    Account.PCPoints = temp;
                    int.TryParse(pcsearch[2], out temp);
                    MaxDesktopPoints = temp;
                }
                catch (Exception)
                {

                    
                }

                try
                {
                    int.TryParse(mobilesearch[1], out temp);
                    Account.MobilePoints = temp;
                    int.TryParse(mobilesearch[2], out temp);
                    MaxMobilePoints = temp;
                }
                catch (Exception)
                {

                    
                }

                try
                {

                    int.TryParse(edgesearch[1], out temp);
                    EdgePoints = temp;
                    int.TryParse(edgesearch[2], out _MaxEdgePoints);
                }
                catch (Exception) { }

            }
            catch (Exception) { return false; }
            return true;
        }
        // Process Daily offers
        public bool Dailyoffers()
        {

            try
            {

                ChromeDriver.Navigate().GoToUrl(DASHBOARD_URL);
                try
                {
                    ChromeDriver.FindElement(By.XPath("//*[@id=\"rewards - error\"]")).Text.Contains("not available");
                    ChromeDriver.Quit();
                    return false;
                }
                catch (Exception) { }
                for (int i = 0; i < 2; i++)
                {
                    var offers = ChromeDriver.FindElementsByXPath("//span[contains(@class, 'mee-icon-AddMedium')]/ancestor::a");
                    if (offers.Count > 0)
                    {
                        foreach (IWebElement link in offers)
                        {
                            Thread.Sleep(1500);
                            var ct = ChromeDriver.WindowHandles[0];
                            link.Click();
                            var nt = ChromeDriver.WindowHandles[1];
                            ChromeDriver.SwitchTo().Window(nt);
                            Thread.Sleep(3500);
                            //driver.ExecuteScript("window.stop()");


                            try
                            {
                                ChromeDriver.FindElement(By.Id("btoption0"));
                                Dailypoll();
                                continue;
                            }
                            catch (Exception) { }
                            try
                            {
                                var btn = ChromeDriver.FindElement(By.Id("rqStartQuiz"));
                                btn.Click();
                                Thread.Sleep(1000);
                                try
                                {
                                    ChromeDriver.FindElement(By.Id("rqAnswerOptionNum0"));
                                    Drag_and_drop_quiz();
                                    continue;
                                }
                                catch (Exception) { }
                                try
                                {
                                    ChromeDriver.FindElement(By.Id("rqAnswerOption0"));
                                    Lightning_quiz();
                                    continue;
                                }
                                catch (Exception) { }

                            }
                            catch (Exception) { }
                            try
                            {
                                ChromeDriver.FindElementByClassName("wk_Circle");
                                click_quiz();
                            }
                            catch (Exception) { }
                            try
                            {
                                explore_daily();

                            }
                            catch (Exception) { }
                        }
                    }
                }

                _IncompleteOffers = ChromeDriver.FindElementsByXPath("//span[contains(@class, 'mee-icon-AddMedium')]").Count;
                return true;
            }
            catch (Exception) { return false; }
        }
        // Simple Visit of site
        private void explore_daily()
        {
            try
            {
                Thread.Sleep(1000);
                var html = ChromeDriver.FindElementByTagName("html");
                for (int i = 0; i < 3; i++)
                {
                    html.SendKeys(Keys.End);
                    Thread.Sleep(50);
                    html.SendKeys(Keys.Home);
                    Thread.Sleep(500);
                }
                go_to_main();
            }
            catch (Exception) { }
        }

        private void click_quiz()
        {
            try
            {
                var rnd = new Random();
                for (int i = 0; i < 20; i++)
                {
                    try
                    {
                        try
                        {
                            if (ChromeDriver.FindElementsByCssSelector(".cico.btCloseBack").Count != 0)
                            {
                                ChromeDriver.FindElementsByCssSelector(".cico.btCloseBack")[0].Click();

                            }
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            var choices = ChromeDriver.FindElementsByClassName("wk_Circle");
                            if (choices.Count != 0)
                            {
                                choices[rnd.Next(choices.Count)].Click();
                                Thread.Sleep(500);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            WebDriverWait wait = new WebDriverWait(ChromeDriver, TimeSpan.FromSeconds(10));
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.ClassName("wk_button")));
                            ChromeDriver.FindElementByClassName("wk_button").Click();
                            Thread.Sleep(2000);
                        }
                        catch (Exception)
                        {
                        }
                        if (ChromeDriver.FindElementsByCssSelector("span[class=\"rw_icon\"]").Count != 0)
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            }
            catch (Exception) { }
            go_to_main();
        }

        private void Lightning_quiz()
        {
            Random rnd = new Random();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    if (ChromeDriver.FindElementsById("rqAnswerOption0").Count != 0)
                    {
                        string firstpage = ChromeDriver.FindElementById("rqAnswerOption0").GetAttribute("data-serpquery");
                        ChromeDriver.Navigate().GoToUrl("https://www.bing.com" + firstpage);
                        Thread.Sleep(1500);
                        for (int j = 0; j < 10; j++)
                        {
                            var answeroption = ChromeDriver.FindElementsByXPath("//*[contains(@id,\"rqAnswerOption\")]");
                            if (answeroption.Count != 0)
                            {
                                var index = rnd.Next(answeroption.Count);
                                answeroption[index].Click();
                                //ChromeDriver.ExecuteScript(string.Format("document.querySelector('#rqAnswerOption{0}').click();", rnd.Next(1,2)));
                                Thread.Sleep(3500);
                            }
                        }
                    }
                    Thread.Sleep(1500);
                    if (ChromeDriver.FindElementsById("quizCompleteContainer").Count != 0)
                    {
                        break;
                    }
                }
                var quiz = ChromeDriver.FindElementsByCssSelector(".cico.btCloseBack");
                if (quiz.Count != 0)
                {
                    quiz[0].Click();
                }
                Thread.Sleep(1500);
            }
            catch (Exception) { }
            go_to_main();
        }

        private void Drag_and_drop_quiz()
        {
            try
            {
                var rnd = new Random();
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        var drag_options = ChromeDriver.FindElementsByClassName("rqOption");
                        var right_answers = ChromeDriver.FindElementsByClassName("correctAnswer");
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
                            Actions builder1 = new Actions(ChromeDriver);
                            IAction dragAndDrop1 = builder1.ClickAndHold(choice_a).MoveToElement(choice_b).Release(choice_a).Build();
                            dragAndDrop1.Perform();
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    Thread.Sleep(500);
                    if (ChromeDriver.FindElementsById("quizCompleteContainer").Count != 0)
                    {
                        break;
                    }
                }
                Thread.Sleep(2500);
                var btn = ChromeDriver.FindElementsByCssSelector(".cico.btCloseBack");
                if (btn.Count != 0)
                {
                    btn[0].Click();
                }
            }
            catch (Exception) { }
            Thread.Sleep(1000);
            go_to_main();
        }

        private void Dailypoll()
        {
            try
            {
                var random = new Random();
                List<string> choices = new List<string>() { "btoption0", "btoption1" };
                ChromeDriver.FindElement(By.Id(choices[random.Next(choices.Count)])).Click();
                Thread.Sleep(500);
                ChromeDriver.FindElement(By.Id("rqCloseBtn")).Click();
                Thread.Sleep(500);
            }
            catch (Exception) { }
            go_to_main();
        }
        public bool BrowserExit()
        {
            var tempid = driverService.ProcessId;
            ChromeDriver.Quit();
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
