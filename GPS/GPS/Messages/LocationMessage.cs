using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Messages
{
    public class LocationMessage
    {
        public Position Position { get; set; }

        public override string ToString()
        {
            return $"La: {Position.Latitude.ToString("N2")}, Lo: {Position.Longitude.ToString("N2")}";
        }
    }
}
