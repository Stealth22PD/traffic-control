using Stealth.Common.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Plugins.TrafficControl.Common
{
    internal static class Globals
    {
        internal static readonly PluginLogger Logger = new PluginLogger(VersionInfo.ProductName);

        internal static bool IsPlayerOnDuty { get; set; } = false;

        private static FileVersionInfo mVersionInfo = null;
        internal static FileVersionInfo VersionInfo
        {
            get
            {
                if (mVersionInfo == null)
                    mVersionInfo = FileVersionInfo.GetVersionInfo(Constants.DLLPath);

                return mVersionInfo;
            }
        }

        internal static Version Version
        {
            get
            {
                return Version.Parse(VersionInfo.FileVersion);
            }
        }
    }
}
