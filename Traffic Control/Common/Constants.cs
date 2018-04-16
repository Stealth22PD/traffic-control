﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stealth.Plugins.TrafficControl.Common
{
    internal static class Constants
    {
        internal const int LCPDFRDownloadID = 7953;
        internal const string DLLPath = @"Plugins\LSPDFR\Traffic Control.dll";

        internal const string ReqCommonVersion = "1.8.6327.38340";
        internal const string ReqRPHVersion = "0.51.1056.10373"; //"0.52.1061.10387";
        internal const string ReqLSPDFRVersion = "0.3.38.5436";
        internal const string ReqRNUIVersion = "1.5.1.0";

        internal const Keys DefaultStopTrafficKey = Keys.U;

        internal const Keys DefaultStopTrafficModKey = Keys.ControlKey;

        internal const Keys DefaultSlowTrafficKey = Keys.I;
        internal const Keys DefaultSlowTrafficModKey = Keys.ControlKey;

        internal const Keys DefaultRemoveZoneKey = Keys.H;
        internal const Keys DefaultRemoveZoneModKey = Keys.ControlKey;

        internal const Keys DefaultRemoveAllZonesKey = Keys.O;
        internal const Keys DefaultRemoveAllZonesModKey = Keys.ControlKey;

        internal const Keys DefaultMenuKey = Keys.F10;
        internal const Keys DefaultMenuModKey = Keys.ControlKey;

        internal const int DefaultRestrictedSpeed = 20;
        internal const int DefaultSpeedZoneRadius = 60;
        internal const bool DefaultPoliceIgnoreRoadblocks = false;
        internal const bool DefaultShortcutKeysEnabled = true;
        internal const bool DefaultBlipsEnabled = true;
    }
}