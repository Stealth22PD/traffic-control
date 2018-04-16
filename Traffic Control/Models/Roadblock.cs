using Rage;
using Stealth.Common.Extensions;
using Stealth.Common.Scripting;
using Stealth.Plugins.TrafficControl.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Plugins.TrafficControl.Models
{
    internal class Roadblock : SpeedZone, ISpeedZone
    {
        public List<Vehicle> AreaVehicles { get; set; } = new List<Vehicle>();
        public bool Active { get; set; } = false;

        internal Roadblock(int speedInMPH, int radius)
        {
            Vector3 location = Game.LocalPlayer.Character.Position;
            CreateSpeedZone(location, speedInMPH, radius);
        }

        internal Roadblock(Vector3 location, int speedInMPH, int radius)
        {
            CreateSpeedZone(location, speedInMPH, radius);
        }

        public override void CreateSpeedZone(Vector3 location, int speedInMPH, int radius)
        {
            ID = Guid.NewGuid();
            Active = true;
            Position = location;
            Radius = radius;
            SpeedLimit = MathHelper.ConvertMilesPerHourToMetersPerSecond(speedInMPH);
            Handle = World.AddSpeedZone(Position, (2 * Radius), MathHelper.ConvertMilesPerHourToMetersPerSecond(30f));

            if (Config.BlipsEnabled)
            {
                Marker = new Blip(Position, Radius)
                {
                    Color = Color.FromArgb(100, Color.Yellow),
                    Alpha = 100
                };
            }

            Process();
        }

        private void Process()
        {
            GameFiber.StartNew(() =>
            {
                while (Active == true)
                {
                    List<Vehicle> vehicles = Vehicles.GetVehiclesNearPosition(Position, Radius, GetEntitiesFlags.ConsiderGroundVehicles);

                    foreach (Vehicle v in vehicles)
                    {
                        if (Active == false)
                        {
                            break;
                        }

                        try
                        {
                            if (v.Exists() && v.HasDriver)
                            {
                                if (AreaVehicles.Contains(v) == false && v.MustObeyPoliceRoadblocks() == true)
                                {
                                    Ped driver = v.Driver;

                                    if (driver.Exists())
                                    {
                                        driver.BlockPermanentEvents = true;
                                        driver.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                                        driver.KeepTasks = true;
                                        driver.MakePersistent();
                                        AreaVehicles.Add(v);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.Logger.LogVerboseDebug("Error processing vehicle for roadblock -- " + ex.Message);
                        }
                    }

                    GameFiber.Sleep(1000);
                    GameFiber.Yield();
                }
            });
        }

        public override void Delete()
        {
            Active = false;

            if (Marker != null && Marker.Exists())
            {
                Marker.Delete();
            }

            foreach (Vehicle v in AreaVehicles)
            {
                if (v != null && v.Exists() && v.HasDriver)
                {
                    Ped driver = v.Driver;

                    if (driver!= null && driver.Exists())
                    {
                        driver.Tasks.Clear();
                        driver.Dismiss();
                    }

                }
            }

            AreaVehicles.Clear();

            try { if (Handle > 0) World.RemoveSpeedZone(Handle); }
            catch { }
        }
    }
}
