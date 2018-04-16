using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Stealth.Plugins.TrafficControl.Common;
using Stealth.Plugins.TrafficControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Stealth.Plugins.TrafficControl
{
    internal static class Driver
    {
        private static List<SpeedZone> SpeedZones = new List<SpeedZone>();
        private static List<Ped> pedList = new List<Ped>();

        private static List<Vehicle> vehList = new List<Vehicle>();

        private static MenuPool MenuPool = new MenuPool();
        private static UIMenu TrafficMenu = null;

        private static UIMenuCheckboxItem CheckboxPoliceIgnore = new UIMenuCheckboxItem("Emergency Vehicles Ignore Roadblocks", Config.PoliceIgnoreRoadblocks, "When enabled, emergency vehicles ignore 'Stop All Traffic' orders.");
        private static UIMenuCheckboxItem CheckboxShortcutsEnabled = new UIMenuCheckboxItem("Enable Shortcut Keys", Config.ShortcutKeysEnabled, "Enable/disable shortcut keybinds.");
        private static UIMenuCheckboxItem CheckboxBlipsEnabled = new UIMenuCheckboxItem("Show Roadblock Blips", Config.BlipsEnabled, "Enable/disable roadblock/speed zone blips.");

        private static List<dynamic> SpeedChoices = new object[] { "15", "20", "25", "30", "35", "40", "45", "50" }.ToList();
        private static List<dynamic> RadiusChoices = new object[] { "20", "40", "60", "80", "100" }.ToList();

        private static UIMenuListItem ListRestrictedSpeed = null;
        private static UIMenuListItem ListSpeedZoneRadius = null;

        private static UIMenuItem ItemStop = new UIMenuItem("Stop All Traffic", "Force all nearby vehicles to stop.");
        private static UIMenuItem ItemSlow = new UIMenuItem("Limit Traffic Speed", "Limit the speed of nearby vehicles.");
        private static UIMenuItem ItemRemove = new UIMenuItem("Remove Nearest", "Remove the roadblock/speed zone closest to you.");
        private static UIMenuItem ItemRemoveAll = new UIMenuItem("Remove All", "Remove all roadblocks/speed zones from the map.");
        private static UIMenuItem ItemSaveINI = new UIMenuItem("Save Options", "Save the current options to the configuration file.");

        internal static void Process()
        {
            Globals.Logger.LogTrivial("Starting process thread...");

            try
            {
                InitMenu();
                InitPoliceSmartRadio();

                Game.FrameRender += Game_FrameRender;

                while (Globals.IsPlayerOnDuty)
                {
                    if (Config.ShortcutKeysEnabled == true)
                    {
                        if (Funcs.IsKeyDown(Config.StopTrafficKey, Config.StopTrafficModKey))
                        {
                            //Add zone, and stop all traffic
                            GameFiber.StartNew(() => StopAllTraffic());
                        }

                        if (Funcs.IsKeyDown(Config.SlowTrafficKey, Config.SlowTrafficModKey))
                        {
                            //Add zone, and limit all traffic speed
                            GameFiber.StartNew(() => LimitTrafficSpeed());
                        }

                        if (Funcs.IsKeyDown(Config.RemoveZoneKey, Config.RemoveZoneModKey))
                        {
                            //Remove nearest zone
                            GameFiber.StartNew(() => RemoveNearestSpeedZone());
                        }

                        if (Funcs.IsKeyDown(Config.RemoveAllZonesKey, Config.RemoveAllZonesModKey))
                        {
                            //Remove all zones
                            GameFiber.StartNew(() => RemoveAllSpeedZones());
                        }
                    }

                    GameFiber.Yield();
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered in process thread -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
            }
            finally
            {
                Globals.Logger.LogTrivial("Stopping process thread...");

                try
                {
                    MenuPool.Remove(TrafficMenu);
                    Game.FrameRender -= Game_FrameRender;
                }
                catch { }
            }
        }

        private static void Game_FrameRender(object sender, GraphicsEventArgs e)
        {
            if (MenuPool.IsAnyMenuOpen() == false)
            {
                if (Config.MenuKey != Keys.None)
                {
                    if (Funcs.IsKeyDown(Config.MenuKey, Config.MenuModKey))
                    {
                        TrafficMenu.RefreshIndex();
                        TrafficMenu.Visible = !TrafficMenu.Visible;
                    }
                }
            }

            MenuPool.ProcessMenus();
        }

        private static void InitMenu()
        {
            List<UIMenuItem> items = new List<UIMenuItem>
            {
                CheckboxPoliceIgnore,
                CheckboxShortcutsEnabled,
                CheckboxBlipsEnabled
            };

            int SpeedIndex = 0;
            if (SpeedChoices.Contains(Config.RestrictedSpeed.ToString()))
            {
                SpeedIndex = SpeedChoices.IndexOf(Config.RestrictedSpeed.ToString());
            }
            else
            {
                SpeedChoices.Add(Config.RestrictedSpeed.ToString());
                SpeedIndex = (SpeedChoices.Count - 1);
            }

            int RadiusIndex = 0;
            if (RadiusChoices.Contains(Config.SpeedZoneRadius.ToString()))
            {
                RadiusIndex = RadiusChoices.IndexOf(Config.SpeedZoneRadius.ToString());
            }
            else
            {
                RadiusChoices.Add(Config.SpeedZoneRadius.ToString());
                RadiusIndex = (RadiusChoices.Count - 1);
            }

            ListRestrictedSpeed = new UIMenuListItem("Restricted Speed in MPH", SpeedChoices, SpeedIndex);
            ListSpeedZoneRadius = new UIMenuListItem("Speed Zone Radius in Metres", RadiusChoices, RadiusIndex);

            items.Add(ListRestrictedSpeed);
            items.Add(ListSpeedZoneRadius);

            items.Add(ItemStop);
            items.Add(ItemSlow);
            items.Add(ItemRemove);
            items.Add(ItemRemoveAll);
            items.Add(ItemSaveINI);

            string mSubtitleText = "Developed By Stealth22";
            TrafficMenu = new UIMenu("Traffic Control", "~b~" + mSubtitleText.ToUpper());

            TrafficMenu.OnCheckboxChange += TrafficMenu_OnCheckboxChange;
            TrafficMenu.OnItemSelect += TrafficMenu_OnItemSelected;
            TrafficMenu.OnListChange += TrafficMenu_OnListChange;

            foreach (UIMenuItem i in items)
            {
                TrafficMenu.AddItem(i);
            }

            TrafficMenu.RefreshIndex();

            MenuPool.Add(TrafficMenu);
        }

        private static void InitPoliceSmartRadio()
        {
            if (Funcs.IsPoliceSmartRadioRunning())
            {
                GameFiber.StartNew(() =>
                {
                    Globals.Logger.LogTrivial("Initializing PoliceSmartRadio integration...");
                    GameFiber.Sleep(5000);

                    Logic.PoliceSmartRadioFuncs.AddActionToButton(() => GameFiber.StartNew(() => StopAllTraffic()), "stop_traffic");
                    Logic.PoliceSmartRadioFuncs.AddActionToButton(() => GameFiber.StartNew(() => LimitTrafficSpeed()), "slow_traffic");
                    Logic.PoliceSmartRadioFuncs.AddActionToButton(() => GameFiber.StartNew(RemoveNearestSpeedZone), new Func<bool>(() => { return SpeedZones.Count > 0; }), "release");
                    Logic.PoliceSmartRadioFuncs.AddActionToButton(() => GameFiber.StartNew(RemoveAllSpeedZones), new Func<bool>(() => { return SpeedZones.Count > 0; }), "release_all");

                    Globals.Logger.LogTrivial("PoliceSmartRadio is now ready for use with Traffic Control");

                }, "InitPoliceSmartRadio");
            }
        }

        internal static Guid StopAllTraffic()
        {
            return StopAllTraffic(Config.PoliceIgnoreRoadblocks);
        }

        internal static Guid StopAllTraffic(bool PoliceIgnoreRoadblocks)
        {
            if (PoliceIgnoreRoadblocks == true)
            {
                return AddRoadblock();
            }
            else
            {
                return AddSpeedZone(0);
            }
        }

        internal static Guid LimitTrafficSpeed()
        {
            return AddSpeedZone(Config.RestrictedSpeed);
        }

        internal static Guid LimitTrafficSpeed(int speed)
        {
            if (speed <= 0)
                speed = Config.RestrictedSpeed;

            return AddSpeedZone(speed);
        }

        private static Guid AddRoadblock()
        {
            try
            {
                Roadblock r = new Roadblock(Game.LocalPlayer.Character.Position, 0, Config.SpeedZoneRadius);
                Funcs.DisplayNotification("Roadblock Added", "All traffic has been stopped");

                SpeedZones.Add(r);

                return r.ID;
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered while creating a roadblock -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
                return Guid.Empty;
            }
        }

        private static Guid AddSpeedZone(int speed)
        {
            try
            {
                SpeedZone s = new SpeedZone(speed, Config.SpeedZoneRadius);

                if (speed == 0)
                    Funcs.DisplayNotification("Roadblock Added", "All traffic has been stopped");
                else
                    Funcs.DisplayNotification("Traffic Limited", string.Format("All traffic limited to a maximum of {0} MPH / {1} KPH", s.SpeedLimitMPH, s.SpeedLimitKPH));

                SpeedZones.Add(s);

                return s.ID;
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered while creating a roadblock -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
                return Guid.Empty;
            }
        }

        private static void RemoveNearestSpeedZone()
        {
            try
            {
                Vector3 playerLocation = Game.LocalPlayer.Character.Position;
                SpeedZone closestSpeedZone = (from x in SpeedZones orderby Vector3.Distance(playerLocation, x.Position) select x).FirstOrDefault();

                if (closestSpeedZone != null)
                {
                    RemoveSpeedZone(closestSpeedZone);
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered while removing closest roadblock -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
            }
        }

        internal static bool RemoveSpeedZone(Guid id)
        {
            try
            {
                SpeedZone speedZone = (from x in SpeedZones where x.ID == id select x).FirstOrDefault();

                if (speedZone != null)
                {
                    return RemoveSpeedZone(speedZone);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered while removing roadblock -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
                return false;
            }
        }

        private static bool RemoveSpeedZone(SpeedZone s)
        {
            try
            {
                if (s != null)
                {
                    s.Delete();

                    if (SpeedZones.Contains(s))
                    {
                        SpeedZones.Remove(s);
                    }

                    Funcs.DisplayNotification("Traffic Released", "Roadblock Removed");

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered while removing a roadblock -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
                return false;
            }
        }

        private static void RemoveAllSpeedZones()
        {
            try
            {
                foreach (SpeedZone s in SpeedZones)
                {
                    if (s != null)
                    {
                        s.Delete();
                    }
                }

                SpeedZones.Clear();

                Funcs.DisplayNotification("Traffic Released", "All roadblocks removed");
            }
            catch (Exception ex)
            {
                Globals.Logger.LogTrivial("Error encountered while removing all roadblocks -- " + ex.Message);
                Globals.Logger.LogTrivialDebug(ex.ToString());
            }
        }

        private static void TrafficMenu_OnItemSelected(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender.Equals(TrafficMenu) == true)
            {
                if (selectedItem.Equals(ItemStop))
                    GameFiber.StartNew(() => StopAllTraffic());

                else if (selectedItem.Equals(ItemSlow))
                    GameFiber.StartNew(() => LimitTrafficSpeed());

                else if (selectedItem.Equals(ItemRemove))
                    GameFiber.StartNew(() => RemoveNearestSpeedZone());

                else if (selectedItem.Equals(ItemRemoveAll))
                    GameFiber.StartNew(() => RemoveAllSpeedZones());

                else if (selectedItem.Equals(ItemSaveINI))
                {
                    GameFiber.StartNew(() =>
                    {
                        Config.UpdateINI();
                        Funcs.DisplayNotification("Config Saved", "Settings have been saved!");
                    });
                }
            }
        }

        private static void TrafficMenu_OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkbox, bool isChecked)
        {
            if (sender.Equals(TrafficMenu) == true)
            {
                if (checkbox.Equals(CheckboxPoliceIgnore))
                    Config.PoliceIgnoreRoadblocks = isChecked;

                else if (checkbox.Equals(CheckboxShortcutsEnabled))
                    Config.ShortcutKeysEnabled = isChecked;

                else if (checkbox.Equals(CheckboxBlipsEnabled))
                    Config.BlipsEnabled = isChecked;
            }
        }

        private static void TrafficMenu_OnListChange(UIMenu sender, UIMenuListItem listItem, int newIndex)
        {
            if (sender.Equals(TrafficMenu) == true)
            {
                if (listItem.Equals(ListRestrictedSpeed))
                    Config.RestrictedSpeed = Convert.ToInt32((string)ListRestrictedSpeed.Items[newIndex]);

                else if (listItem.Equals(ListSpeedZoneRadius))
                    Config.SpeedZoneRadius = Convert.ToInt32((string)ListSpeedZoneRadius.Items[newIndex]);
            }
        }
    }
}