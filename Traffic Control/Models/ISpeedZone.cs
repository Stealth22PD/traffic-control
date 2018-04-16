using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Plugins.TrafficControl.Models
{
    public interface ISpeedZone
    {
        Guid ID { get; set; }
        uint Handle { get; set; }
        Vector3 Position { get; set; }
        int Radius { get; set; }
        Blip Marker { get; set; }
        float SpeedLimit { get; set; }
        int SpeedLimitMPH { get; }
        int SpeedLimitKPH { get; }

        void CreateSpeedZone(Vector3 location, int speedInMPH, int radius);
        void Delete();
    }
}
