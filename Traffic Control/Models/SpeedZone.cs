using Rage;
using Stealth.Plugins.TrafficControl.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Plugins.TrafficControl.Models
{
    internal class SpeedZone : ISpeedZone
    {
        internal SpeedZone() { }

        internal SpeedZone(int speedInMPH, int radius)
        {
            Vector3 location = Game.LocalPlayer.Character.Position;
            CreateSpeedZone(location, speedInMPH, radius);
        }

        internal SpeedZone(Vector3 location, int speedInMPH, int radius)
        {
            CreateSpeedZone(location, speedInMPH, radius);
        }

        public virtual void CreateSpeedZone(Vector3 location, int speedInMPH, int radius)
        {
            ID = Guid.NewGuid();
            Position = location;
            Radius = radius;
            SpeedLimit = MathHelper.ConvertMilesPerHourToMetersPerSecond(speedInMPH);
            Handle = World.AddSpeedZone(Position, Radius, SpeedLimit);
            
            if (Config.BlipsEnabled)
            {
                Marker = new Blip(Position, Radius)
                {
                    Color = Color.FromArgb(100, Color.Yellow),
                    Alpha = 100
                };
            }
        }

        public virtual void Delete()
        {
            if (Marker != null && Marker.Exists())
            {
                Marker.Delete();
            }

            try { if (Handle > 0) World.RemoveSpeedZone(Handle); }
            catch { }
        }

        public Guid ID { get; set; } = Guid.Empty;
        public uint Handle { get; set; } = 0;
        public Vector3 Position { get; set; } = Vector3.Zero;
        public int Radius { get; set; } = 0;
        public Blip Marker { get; set; } = null;
        public float SpeedLimit { get; set; } = 0;

        public int SpeedLimitMPH
        {
            get { return Convert.ToInt32(Math.Round(MathHelper.ConvertMetersPerSecondToMilesPerHour(SpeedLimit), MidpointRounding.AwayFromZero)); }
        }

        public int SpeedLimitKPH
        {
            get { return MathHelper.ConvertMetersPerSecondToKilometersPerHourRounded(SpeedLimit); }
        }
    }
}
