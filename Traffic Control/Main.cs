using LSPD_First_Response.Mod.API;
using Stealth.Plugins.TrafficControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Stealth.Common.Functions;
using Rage;

namespace Stealth.Plugins.TrafficControl
{
    internal sealed class Main : Plugin
    {
        public override void Initialize()
        {
            Config.Init();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        private Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
                {
                    return assembly;
                }
            }
            return null;
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (!onDuty)
            {
                Globals.IsPlayerOnDuty = false;
                return;
            }

            if (!Funcs.PreloadChecks())
            {
                return;
            }

            StartPlugin(onDuty);
        }

        private static void StartPlugin(bool onDuty)
        {
            Globals.Logger.LogTrivial("Starting Traffic Control...");

            Globals.IsPlayerOnDuty = onDuty;

            if (onDuty)
            {
                GameFiber.StartNew(delegate
                {
                    Globals.Logger.LogTrivial(String.Format("{0} v{1} has been loaded!", Globals.VersionInfo.ProductName, Globals.VersionInfo.FileVersion));
                    Funcs.CheckForUpdates();
                });

                GameFiber.StartNew(Driver.Process);
            }
        }

        public override void Finally()
        {
            Globals.IsPlayerOnDuty = false;
        }
    }
}
