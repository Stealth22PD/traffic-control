using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stealth.Plugins.TrafficControl.Common
{
    internal static class Config
    {
        internal const string INIFileName = "Traffic Control.ini";
        internal const string INIFilePath = "Plugins\\LSPDFR\\" + INIFileName;
        private static InitializationFile mINIFile = new InitializationFile(INIFilePath);

        internal static Keys StopTrafficKey { get; set; } = Constants.DefaultStopTrafficKey;
        internal static Keys StopTrafficModKey { get; set; } = Constants.DefaultStopTrafficModKey;

        internal static Keys SlowTrafficKey { get; set; } = Constants.DefaultSlowTrafficKey;
        internal static Keys SlowTrafficModKey { get; set; } = Constants.DefaultSlowTrafficModKey;

        internal static Keys RemoveZoneKey { get; set; } = Constants.DefaultRemoveZoneKey;
        internal static Keys RemoveZoneModKey { get; set; } = Constants.DefaultRemoveZoneModKey;

        internal static Keys RemoveAllZonesKey { get; set; } = Constants.DefaultRemoveAllZonesKey;
        internal static Keys RemoveAllZonesModKey { get; set; } = Constants.DefaultRemoveAllZonesModKey;

        internal static Keys MenuKey { get; set; } = Constants.DefaultMenuKey;
        internal static Keys MenuModKey { get; set; } = Constants.DefaultMenuModKey;

        internal static int RestrictedSpeed { get; set; } = Constants.DefaultRestrictedSpeed;
        internal static int SpeedZoneRadius { get; set; } = Constants.DefaultSpeedZoneRadius;
        internal static bool PoliceIgnoreRoadblocks { get; set; } = Constants.DefaultPoliceIgnoreRoadblocks;
        internal static bool ShortcutKeysEnabled { get; set; } = Constants.DefaultShortcutKeysEnabled;
        internal static bool BlipsEnabled { get; set; } = Constants.DefaultBlipsEnabled;

        internal static void Init()
        {
            if (mINIFile.Exists() == false)
            {
                CreateINI();
            }

            ReadINI();
            Globals.Logger.LogTrivial("Settings loaded");
        }

        private static void CreateINI()
        {
            mINIFile.Create();

            // Settings
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.RestrictedSpeed.ToString(), Constants.DefaultRestrictedSpeed);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.SpeedZoneRadius.ToString(), Constants.DefaultSpeedZoneRadius);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.PoliceIgnoreRoadblocks.ToString(), Constants.DefaultPoliceIgnoreRoadblocks);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.ShortcutKeysEnabled.ToString(), Constants.DefaultShortcutKeysEnabled);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.BlipsEnabled.ToString(), Constants.DefaultBlipsEnabled);

            // Keys
            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.StopTrafficKey.ToString(), Constants.DefaultStopTrafficKey);
            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.StopTrafficModKey.ToString(), Constants.DefaultStopTrafficModKey);

            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.SlowTrafficKey.ToString(), Constants.DefaultSlowTrafficKey);
            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.SlowTrafficModKey.ToString(), Constants.DefaultSlowTrafficModKey);

            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.RemoveZoneKey.ToString(), Constants.DefaultRemoveZoneKey);
            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.RemoveZoneModKey.ToString(), Constants.DefaultRemoveZoneModKey);

            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.RemoveAllZonesKey.ToString(), Constants.DefaultRemoveAllZonesKey);
            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.RemoveAllZonesModKey.ToString(), Constants.DefaultRemoveAllZonesModKey);

            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.MenuKey.ToString(), Constants.DefaultMenuKey);
            mINIFile.Write(ECfgSections.KEYS.ToString(), EKeys.MenuModKey.ToString(), Constants.DefaultMenuModKey);
        }

        internal static void UpdateINI()
        {
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.RestrictedSpeed.ToString(), RestrictedSpeed);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.SpeedZoneRadius.ToString(), SpeedZoneRadius);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.PoliceIgnoreRoadblocks.ToString(), PoliceIgnoreRoadblocks);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.ShortcutKeysEnabled.ToString(), ShortcutKeysEnabled);
            mINIFile.Write(ECfgSections.SETTINGS.ToString(), ESettings.BlipsEnabled.ToString(), BlipsEnabled);
        }

        private static void ReadINI()
        {
            // Settings
            RestrictedSpeed = mINIFile.ReadInt32(ECfgSections.SETTINGS.ToString(), ESettings.RestrictedSpeed.ToString(), Constants.DefaultRestrictedSpeed);
            SpeedZoneRadius = mINIFile.ReadInt32(ECfgSections.SETTINGS.ToString(), ESettings.SpeedZoneRadius.ToString(), Constants.DefaultSpeedZoneRadius);
            PoliceIgnoreRoadblocks = mINIFile.ReadBoolean(ECfgSections.SETTINGS.ToString(), ESettings.PoliceIgnoreRoadblocks.ToString(), Constants.DefaultPoliceIgnoreRoadblocks);
            ShortcutKeysEnabled = mINIFile.ReadBoolean(ECfgSections.SETTINGS.ToString(), ESettings.ShortcutKeysEnabled.ToString(), Constants.DefaultShortcutKeysEnabled);
            BlipsEnabled = mINIFile.ReadBoolean(ECfgSections.SETTINGS.ToString(), ESettings.BlipsEnabled.ToString(), Constants.DefaultBlipsEnabled);

            // Keys
            StopTrafficKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.StopTrafficKey.ToString(), Constants.DefaultStopTrafficKey);
            StopTrafficModKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.StopTrafficModKey.ToString(), Constants.DefaultStopTrafficModKey);

            SlowTrafficKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.SlowTrafficKey.ToString(), Constants.DefaultSlowTrafficKey);
            SlowTrafficModKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.SlowTrafficModKey.ToString(), Constants.DefaultSlowTrafficModKey);

            RemoveZoneKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.RemoveZoneKey.ToString(), Constants.DefaultRemoveZoneKey);
            RemoveZoneModKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.RemoveZoneModKey.ToString(), Constants.DefaultRemoveZoneModKey);

            RemoveAllZonesKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.RemoveAllZonesKey.ToString(), Constants.DefaultRemoveAllZonesKey);
            RemoveAllZonesModKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.RemoveAllZonesModKey.ToString(), Constants.DefaultRemoveAllZonesModKey);

            MenuKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.MenuKey.ToString(), Constants.DefaultMenuKey);
            MenuModKey = mINIFile.ReadEnum<Keys>(ECfgSections.KEYS.ToString(), EKeys.MenuModKey.ToString(), Constants.DefaultMenuModKey);
        }

        private enum ECfgSections
        {
            SETTINGS,
            KEYS
        }

        private enum ESettings
        {
            RestrictedSpeed,
            SpeedZoneRadius,
            PoliceIgnoreRoadblocks,
            ShortcutKeysEnabled,
            BlipsEnabled
        }

        private enum EKeys
        {
            StopTrafficKey,
            StopTrafficModKey,
            SlowTrafficKey,
            SlowTrafficModKey,
            RemoveZoneKey,
            RemoveZoneModKey,
            RemoveAllZonesKey,
            RemoveAllZonesModKey,
            MenuKey,
            MenuModKey
        }
    }
}
