using DotNetTor.SocksPort;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MicrosoftRewardsBot.BotClasses
{
    public class Tor : ObservableObject
    {
        private Process process = new Process();
        private ObservableCollection<string> torLog = new ObservableCollection<string>();
        private string TorAddress = Properties.Resources.TorAddress;
        private string UserId;
        private int ProxyPort = 9050;
        private bool isConnected = false;
        private bool isConnectionValid = false;
        private bool _ErrorConnecting = false;
        private string iPAddress;

        public ObservableCollection<string> TorLog { get => torLog; set => SetProperty(ref torLog, value); }
        public bool IsConnected { get => isConnected; set => SetProperty(ref isConnected, value); }
        public bool IsConnectionValid { get => isConnectionValid; set => SetProperty(ref isConnectionValid, value); }
        public string IPAddress { get => iPAddress; set => SetProperty(ref iPAddress, value); }
        public bool ErrorConnecting { get => _ErrorConnecting; set => SetProperty(ref _ErrorConnecting, value); }

        public Tor(int userId, int proxyPort = 9050)
        {
            UserId = userId.ToString();
            ProxyPort = proxyPort;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);
        }

        private void OnAppExit(object sender, EventArgs e)
        {
            ExitTor();
        }
        public async Task<bool> ConnectAsync()
        {
            LaunchProcess();
            Thread.Sleep(1000);
            if (await ConnectionValidatorAsync())
            {
                return true;
            }

            return false;

        }

        public bool LaunchProcess()
        {
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            process.Exited += new EventHandler(process_Exited);

            process.StartInfo.FileName = TorAddress;
            process.StartInfo.Arguments = @"-f "".\Tor\" + ProxyPort + @"\torrc""";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return true;

        }
        void process_Exited(object sender, EventArgs e)
        {
            

        }



        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            

        }



        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            if (e.Data != null)
            {
                if (e.Data.Contains("100%"))
                {

                    IsConnected = true;


                }

            }

        }

        async Task IPValidatorAsync()
        {

            using (var httpClient = new HttpClient(new SocksPortHandler("127.0.0.1", socksPort: ProxyPort)))
            {
                var message = await httpClient.GetAsync("http://icanhazip.com/");
                IPAddress = message.Content.ReadAsStringAsync().Result;
                IPAddress = IPAddress.Remove(IPAddress.Length - 2);
                IsConnected = true;
            }

            if (!await CheckCountry(IPAddress, ProxyPort))
            {

                await ChangeIPAsync(ProxyPort);
            }
        }

        // Changes tor Connection IP
        public async Task<bool> ChangeIPAsync(int id = 9050)
        {

            string curent_ip = "";
            string new_ip = "";
            bool check = true;
            while (curent_ip == new_ip || check)
            {
                curent_ip = new_ip;
                using (var httpClient = new HttpClient(new SocksPortHandler("127.0.0.1", socksPort: id)))
                {

                    var controlPortClient = new DotNetTor.ControlPort.Client("127.0.0.1", controlPort: id + 1, password: "ILoveBitcoin21");
                    controlPortClient.ChangeCircuitAsync().Wait();

                    var message = await httpClient.GetAsync("http://icanhazip.com/");
                    new_ip = message.Content.ReadAsStringAsync().Result;
                    new_ip = new_ip.Remove(new_ip.Length - 2);
                    IPAddress = new_ip;
                    check = await CheckCountry(new_ip, id);
                    if (check)
                    {

                        return true;
                    }
                    TorLog.Add("Waiting 10s before Requesting new Circuit");
                    Thread.Sleep(10000);

                }
            }
            return true;
        }

        // Checks IP Address to be from US
        private async Task<bool> CheckCountry(string ip, int id = 9050)
        {

            using (var httpClient = new HttpClient(new SocksPortHandler("127.0.0.1", socksPort: id)))
            {
                var cl = await httpClient.GetAsync("http://ip-api.com/line/" + ip + "?fields=countryCode");
                var scl = await httpClient.GetAsync("https://ipwhois.app/line/" + ip + "?objects=country");
                if (cl.Content.ReadAsStringAsync().Result == "US\n" && string.Equals(scl.Content.ReadAsStringAsync().Result, "United States"))
                {
                    IsConnectionValid = true;
                    return true;
                }
            }
            return false;

        }

        private async Task<bool> ConnectionValidatorAsync()
        {
            while (!isConnectionValid)
            {
                await IPValidatorAsync();
            }

            return true;
        }
        public void ExitTor()
        {
            try
            {
                process.Kill();
                IsConnected = false;
            }
            catch (Exception) { }

        }
    }
}
