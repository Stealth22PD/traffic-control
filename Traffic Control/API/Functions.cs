using Stealth.Plugins.TrafficControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stealth.Plugins.TrafficControl.API
{
    /// <summary>
    /// Public class that allows other LSPDFR plugins to call Traffic Control functions. Please ensure to follow Albo1125's guide on calling LSPDFR plugin API's.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Stops all traffic near the player, using the user's configured value for the "PoliceIgnoreRoadblocks" setting.
        /// </summary>
        /// <returns>A GUID containing the ID of the roadblock. If Guid.Empty is returned, the operation failed.</returns>
        public static Guid StopAllTraffic()
        {
            return StopAllTraffic(Config.PoliceIgnoreRoadblocks);
        }

        /// <summary>
        /// Stops all traffic near the player, OVERRIDING the user's configured value for the "PoliceIgnoreRoadblocks" setting.
        /// </summary>
        /// <param name="PoliceIgnoreRoadblocks">Whether or not police/emergency vehicles should be exempt from 'Stop All Traffic' orders.</param>
        /// <returns>A GUID containing the ID of the roadblock. If Guid.Empty is returned, the operation failed.</returns>
        public static Guid StopAllTraffic(bool PoliceIgnoreRoadblocks)
        {
            return Driver.StopAllTraffic(PoliceIgnoreRoadblocks);
        }

        /// <summary>
        /// Limits the speed of all traffic (including emergency vehicles) to the speed set in the user's configuration.
        /// </summary>
        /// <returns>A GUID containing the ID of the roadblock. If Guid.Empty is returned, the operation failed.</returns>
        public static Guid LimitTrafficSpeed()
        {
            return Driver.LimitTrafficSpeed();
        }

        /// <summary>
        /// Limits the speed of all traffic (including emergency vehicles) to the speed specified by the calling function.
        /// </summary>
        /// <param name="speed">The speed (in miles per hour) to limit traffic to.</param>
        /// <returns>A GUID containing the ID of the roadblock. If Guid.Empty is returned, the operation failed.</returns>
        public static Guid LimitTrafficSpeed(int speed)
        {
            return Driver.LimitTrafficSpeed(speed);
        }

        /// <summary>
        /// Removes a roadblock, and releases traffic to normal speeds.
        /// </summary>
        /// <param name="id">The GUID of the roadblock, which is returned by 'StopAllTraffic' and 'LimitTrafficSpeed'.</param>
        /// <returns>True if the operation succeeded, and false if it failed.</returns>
        public static bool RemoveSpeedZone(Guid id)
        {
            return Driver.RemoveSpeedZone(id);
        }
    }
}
