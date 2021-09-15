using System;
using System.Diagnostics;
using System.Reflection;

using MicrosoftRewardsBot.Contracts.Services;

namespace MicrosoftRewardsBot.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public ApplicationInfoService()
        {
        }

        public Version GetVersion()
        {
            // Set the app version in MicrosoftRewardsBot > Properties > Package > PackageVersion
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            return new Version(version);
        }
    }
}
