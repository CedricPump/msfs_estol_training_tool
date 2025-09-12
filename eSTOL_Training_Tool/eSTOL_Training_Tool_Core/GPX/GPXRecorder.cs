﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using eSTOL_Training_Tool;
using eSTOL_Training_Tool_Core.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace eSTOL_Training_Tool_Core.GPX
{
    class GPXRecorder
    {
        static readonly int maxcount = 10000;
        private List<(Telemetrie telemetrie, DateTime timestamp)> geoCoordinates = new();
        private DateTime last = DateTime.MinValue;
        private TimeSpan interval = TimeSpan.FromMilliseconds(250);
        private Config config = Config.GetInstance();

        public void Append(Telemetrie telemetrie)
        {
            var now = DateTime.Now;
            if (now - last > interval)
            {
                geoCoordinates.Add((telemetrie, DateTime.UtcNow));
                last = now;
            }
            if (geoCoordinates.Count > maxcount)
            {
                geoCoordinates.RemoveAt(0);
            }
            // Console.WriteLine(geoCoordinates.Count);
        }

        public void Reset()
        {
            geoCoordinates.Clear();
        }

        public void Save(string user, string planetype)
        {
            ExportGPX($"{user}_{planetype}_{DateTime.UtcNow:yyyy-MM-ddTHHmmssZ}", planetype);
            ExportCSV($"{user}_{planetype}_{DateTime.UtcNow:yyyy-MM-ddTHHmmssZ}");
        }

        public void ExportGPX(string filename, string planetype)
        {
            if (geoCoordinates.Count < 2)
                return; // Not enough data

            string path = Path.Combine(config.RecordingExportPath, $"{filename}.gpx");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var start = geoCoordinates[0];
            var end = geoCoordinates[^1];
            string creationDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
            string localStart = start.timestamp.ToLocalTime().ToString("HH:mm");
            string localEnd = end.timestamp.ToLocalTime().ToString("HH:mm");

            using (XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8
            }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
                writer.WriteAttributeString("version", "1.1");
                writer.WriteAttributeString("creator", "eSTOL_Training_tool");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi", "schemaLocation", null,
                    "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");

                // Metadata
                writer.WriteStartElement("metadata");
                writer.WriteElementString("name", filename);
                writer.WriteStartElement("desc");
                writer.WriteCData($"Creation date: {creationDate}\nFlight number: 0\nStart (local time): {localStart}\nEnd (local time): {localEnd}\nCategory: Airplane");
                writer.WriteEndElement(); // desc
                writer.WriteEndElement(); // metadata

                // Waypoints
                WriteWaypoint(writer, start.telemetrie.Position, start.timestamp, "CUSTD", "Departure");
                WriteWaypoint(writer, end.telemetrie.Position, end.timestamp, "CUSTA", "Arrival");

                // Track
                writer.WriteStartElement("trk");
                writer.WriteStartElement("name");
                writer.WriteCData(planetype);
                writer.WriteEndElement(); // name

                writer.WriteStartElement("desc");
                writer.WriteCData(@"Category: Airplane");
                writer.WriteEndElement(); // desc

                writer.WriteStartElement("trkseg");

                foreach (var (telemetrie, timestamp) in geoCoordinates)
                {
                    writer.WriteStartElement("trkpt");
                    writer.WriteAttributeString("lat", telemetrie.Position.Latitude.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("lon", telemetrie.Position.Longitude.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString("ele", telemetrie.Altitude.ToString("F2", CultureInfo.InvariantCulture));
                    writer.WriteElementString("time", timestamp.ToString("o"));
                    writer.WriteEndElement(); // trkpt
                }

                writer.WriteEndElement(); // trkseg
                writer.WriteEndElement(); // trk
                writer.WriteEndElement(); // gpx
                writer.WriteEndDocument();
            }
        }

        private void WriteWaypoint(XmlWriter writer, GeoCoordinate coord, DateTime timestamp, string name, string desc)
        {
            writer.WriteStartElement("wpt");
            writer.WriteAttributeString("lat", coord.Latitude.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("lon", coord.Longitude.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ele", coord.Altitude.ToString("F2", CultureInfo.InvariantCulture));
            writer.WriteElementString("time", timestamp.ToString("o"));
            writer.WriteElementString("geoidheight", "-17.30"); // Optional, hardcoded for now
            writer.WriteElementString("name", name);
            writer.WriteElementString("desc", desc);
            writer.WriteEndElement(); // wpt
        }


        public void ExportDollyCSV(string filename = null)
        {
            if (geoCoordinates.Count < 2)
                return;

            filename ??= $"flight_{DateTime.UtcNow:yyyy-MM-ddTHHmmssZ}";
            string path = Path.Combine(config.RecordingExportPath, $"{filename}.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("Timestamp,UTC,Latitude,Longitude,Altitude,Speed,Pitch,Bank,Heading");

                var baseTime = geoCoordinates[0].timestamp;

                foreach (var (telemetrie, timestamp) in geoCoordinates)
                {
                    int milliseconds = (int)(timestamp - baseTime).TotalMilliseconds;

                    string utc = timestamp.ToString("o", CultureInfo.InvariantCulture);
                    string lat = telemetrie.Position.Latitude.ToString("F6", CultureInfo.InvariantCulture);
                    string lon = telemetrie.Position.Longitude.ToString("F6", CultureInfo.InvariantCulture);
                    string alt = Math.Round(telemetrie.Altitude).ToString(CultureInfo.InvariantCulture);
                    string speed = Math.Round(telemetrie.GroundSpeed).ToString(CultureInfo.InvariantCulture);
                    string pitch = Math.Round(telemetrie.pitch).ToString(CultureInfo.InvariantCulture);
                    string bank = Math.Round(telemetrie.bank).ToString(CultureInfo.InvariantCulture);
                    string heading = Math.Round(telemetrie.Heading).ToString(CultureInfo.InvariantCulture);

                    writer.WriteLine($"{milliseconds},{utc},{lat},{lon},{alt},{speed},{pitch},{bank},{heading}");
                }
            }
        }

        public void ExportCSV(string filename = null)
        {
            filename ??= $"flight_{DateTime.UtcNow:yyyy-MM-ddTHHmmssZ}";
            string path = Path.Combine(config.RecordingExportPath, $"{filename}.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using var writer = new StreamWriter(path, false, Encoding.UTF8);

            var baseTime = geoCoordinates[0].timestamp;

            writer.WriteLine("Milliseconds,Latitude,Longitude,Altitude,Pitch,Bank,GyroHeading,TrueHeading,MagneticHeading,VelocityBodyX,VelocityBodyY,VelocityBodyZ,AileronPosition,ElevatorPosition,RudderPosition,ElevatorTrimPosition,AileronTrimPercent,RudderTrimPercent,FlapsHandleIndex,TrailingEdgeFlapsLeftPercent,TrailingEdgeFlapsRightPercent,LeadingEdgeFlapsLeftPercent,LeadingEdgeFlapsRightPercent,ThrottleLeverPosition1,ThrottleLeverPosition2,ThrottleLeverPosition3,ThrottleLeverPosition4,PropellerLeverPosition1,PropellerLeverPosition2,PropellerLeverPosition3,PropellerLeverPosition4,SpoilerHandlePosition,GearHandlePosition,WaterRudderHandlePosition,BrakeLeftPosition,BrakeRightPosition,BrakeParkingPosition,LightTaxi,LightLanding,LightStrobe,LightBeacon,LightNav,LightWing,LightLogo,LightRecognition,LightCabin,SimulationRate,AbsoluteTime,AltitudeAboveGround,IsOnGround,WindVelocity,WindDirection,GForce,TouchdownNormalVelocity,WingFlexPercent1,WingFlexPercent2,WingFlexPercent3,WingFlexPercent4,TrueAirspeed,IndicatedAirspeed,MachAirspeed,GpsGroundSpeed,GroundSpeed,HeadingIndicator,AIPitch,AIBank,EngineManifoldPressure1,EngineManifoldPressure2,EngineManifoldPressure3,EngineManifoldPressure4,TurnCoordinatorBall,HsiCDI,StallWarning,RotationVelocityBodyX,RotationVelocityBodyY,RotationVelocityBodyZ,AccelerationBodyX,AccelerationBodyY,AccelerationBodyZ");
            // writer.WriteLine("Milliseconds,Latitude,Longitude,Altitude,Pitch,Bank,TrueHeading");
            foreach (var (telemetrie, timestamp) in geoCoordinates)
            {
                int milliseconds = (int)(timestamp - baseTime).TotalMilliseconds;

                var line = string.Join(",", new[]
{
    milliseconds.ToString(),
    telemetrie.Position.Latitude.ToString("F15", CultureInfo.InvariantCulture),
    telemetrie.Position.Longitude.ToString("F15", CultureInfo.InvariantCulture),
    (telemetrie.Position.Altitude * 3.28084).ToString("F2", CultureInfo.InvariantCulture),
    telemetrie.pitch.ToString("F2", CultureInfo.InvariantCulture),
    telemetrie.bank.ToString("F2", CultureInfo.InvariantCulture),
    "148.2872388631069", // GyroHeading
    telemetrie.Heading.ToString("F2", CultureInfo.InvariantCulture),
    "147.48626043109184", // MagneticHeading
    "0", // VelocityBodyX
    "0", // VelocityBodyY
    "0", // VelocityBodyZ
    "0.0", // AileronPosition
    "0.0", // ElevatorPosition
    "0.0", // RudderPosition
    "0.0", // ElevatorTrimPosition
    "0", // AileronTrimPercent
    "0", // RudderTrimPercent
    "1", // FlapsHandleIndex
    "0.25", // TrailingEdgeFlapsLeftPercent
    "0.25", // TrailingEdgeFlapsRightPercent
    "0", // LeadingEdgeFlapsLeftPercent
    "0", // LeadingEdgeFlapsRightPercent
    "0", // ThrottleLeverPosition1
    "0", // ThrottleLeverPosition2
    "0", // ThrottleLeverPosition3
    "0", // ThrottleLeverPosition4
    "0", // PropellerLeverPosition1
    "0", // PropellerLeverPosition2
    "0", // PropellerLeverPosition3
    "0", // PropellerLeverPosition4
    "0", // SpoilerHandlePosition
    "0", // GearHandlePosition
    "1", // WaterRudderHandlePosition
    "-1", // BrakeLeftPosition
    "0.9999986886978149", // BrakeRightPosition
    "0.9999986886978149", // BrakeParkingPosition
    "1", // LightTaxi
    "0", // LightLanding
    "0", // LightStrobe
    "0", // LightBeacon
    "1", // LightNav
    "0", // LightWing
    "0", // LightLogo
    "0", // LightRecognition
    "0", // LightCabin
    "1", // SimulationRate
    "63886122215.27931", // AbsoluteTime
    (telemetrie.AltitudeAGL  * 3.28084).ToString("F2", CultureInfo.InvariantCulture), // AltitudeAboveGround
    "1", // IsOnGround
    "8.999655226837202", // WindVelocity
    "269.9947204589844", // WindDirection
    "0.9653272808567918", // GForce
    "0", // TouchdownNormalVelocity
    "-0.4968649446964264", // WingFlexPercent1
    "-0.4968528747558594", // WingFlexPercent2
    "-0.4987638294696808", // WingFlexPercent3
    "-0.49875974655151367", // WingFlexPercent4
    "0", // TrueAirspeed
    "0", // IndicatedAirspeed
    "0.0041549536399543285", // MachAirspeed
    "0", // GpsGroundSpeed
    "0", // GroundSpeed
    telemetrie.Heading.ToString("F2", CultureInfo.InvariantCulture), // HeadingIndicator
    telemetrie.pitch.ToString("F2", CultureInfo.InvariantCulture), // AIPitch
    telemetrie.bank.ToString("F2", CultureInfo.InvariantCulture), // AIBank
    "2.4870328895478178", // EngineManifoldPressure1
    "0", // EngineManifoldPressure2
    "0", // EngineManifoldPressure3
    "0", // EngineManifoldPressure4
    "0", // TurnCoordinatorBall
    "0", // HsiCDI
    "0", // StallWarning
    "0", // RotationVelocityBodyX
    "0", // RotationVelocityBodyY
    "0", // RotationVelocityBodyZ
    "0", // AccelerationBodyX
    "0", // AccelerationBodyY
    "0", // AccelerationBodyZ
});

                writer.WriteLine(line);
            }
        }


    }
}
