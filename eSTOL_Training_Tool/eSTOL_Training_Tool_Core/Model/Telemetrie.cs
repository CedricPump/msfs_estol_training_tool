using Newtonsoft.Json;
using System;
using System.Device.Location;

namespace eSTOL_Training_Tool
{
    public class Telemetrie
    {

        /// <summary>
        /// Position in lat, long, height
        /// </summary>
        [JsonProperty("position")]
        public GeoCoordinate Position { get; set; }

        /// <summary>
        /// height AGL
        /// </summary>
        [JsonProperty("height")]
        public double Height { get; set; } = 0;

        /// <summary>
        /// physical Altitude ASL in ft
        /// </summary>
        [JsonProperty("alt")]
        public double Altitude { get; set; }

        /// <summary>
        /// physical Altitude AGL in ft
        /// </summary>
        [JsonProperty("alt_agl")]
        public double AltitudeAGL { get; set; }

        /// <summary>
        /// Ground speed in knots
        /// </summary>
        [JsonProperty("ground_speed")]
        public double GroundSpeed { get; set; }

        /// <summary>
        /// Heading in degrees
        /// </summary>
        [JsonProperty("heading")]
        public double Heading { get; set; }

        /// <summary>
        /// velocity X in m/s
        /// relative to aircraft
        /// </summary>
        [JsonProperty("vx")]
        public double vX { get; set; }

        /// <summary>
        /// velocity Y in m/s
        /// relative to aircraft
        /// </summary>
        [JsonProperty("vy")]
        public double vY { get; set; }

        /// <summary>
        /// velocity Z in m/s
        /// relative to aircraft
        /// </summary>
        [JsonProperty("vz")]
        public double vZ { get; set; }

        [JsonProperty("pitch")]
        public double pitch { get; set; } = 0;
        [JsonProperty("bank")]
        public double bank { get; set; } = 0;

        public double verticalSpeed { get; set; } = 0;

        public double gForce { get; set; } = 0;

        public double mainWheelRPM { get; set; } = 0;
        public double centerWheelRPM { get; set; } = 0;

        public override string ToString()
        {
            return $"[{GeoUtils.ConvertToDMS(Position)}], {Math.Round(Altitude)} ft, {Math.Round(Heading)}°, {Math.Round(GroundSpeed)} knts";
        }
    }
}
