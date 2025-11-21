using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using eSTOL_Training_Tool_Core.Model;

namespace eSTOL_Training_Tool
{
    public class STOLData 
    {
        public STOLData() { }

        // meta
        public string planeType = "";
        public string planeKey = "";
        public string user = "";
        public Preset preset = null;
        public string sessionKey = "none";

        // Positions
        public GeoCoordinate InitialPosition = null;
        public GeoCoordinate TakeoffPosition = null;
        public GeoCoordinate TouchdownPosition = null;
        public GeoCoordinate StopPosition = null;
        public int PatternAlt = 0;

        // Meta
        public double? InitialPitch = null;
        public double? InitialHeading = null;
        public double? TouchdownGroundSpeed = null;
        public double? TouchdownVerticalSpeed = null;
        public double? TouchdownPitch = null;
        public double? TouchdownGs = null;
        public double? maxSpin = null;
        public double? minPitch = null;
        public double? maxBank = null;

        // Timing
        public DateTime? StartTime = null;
        public DateTime? TakeoffTime = null;
        public DateTime? TouchdownTime = null;
        public DateTime? StopTime = null;

        // wind
        public double takeoffWindSpeed = 0;
        public double takeoffWindDirection = 0;
        public double landingWindSpeed = 0;
        public double landingWindDirection = 0;

        public List<STOLDeviation> deviations = new List<STOLDeviation>();



        public bool IsInit() { return InitialPosition != null && InitialHeading != null; }

        public double GetTakeoffDistance() 
        {
            if (InitialPosition == null || TakeoffPosition == null) throw new ArgumentException("Position null");

            return GeoUtils.GetDistanceAlongAxis(InitialPosition, TakeoffPosition, InitialHeading.Value).Item1;
        }

        public double GetTouchdownDistance()
        {
            if (InitialPosition == null || TouchdownPosition == null) throw new ArgumentException("Position null");

            return GeoUtils.GetDistanceAlongAxis(InitialPosition, TouchdownPosition, InitialHeading.Value).Item1;
        }

        public double GetLandingDistance()
        {
            if (InitialPosition == null || StopPosition == null) throw new ArgumentException("Position null");

            return GeoUtils.GetDistanceAlongAxis(InitialPosition, StopPosition, InitialHeading.Value).Item1;
        }

        public double GetStoppingDistance() 
        {
            if (TouchdownPosition == null || StopPosition == null) throw new ArgumentException("Position null");

            return GeoUtils.GetDistanceAlongAxis(TouchdownPosition, StopPosition, InitialHeading.Value).Item1;
        }

        public string GetInitialPosHash()
        {
            return GetDeterministicHash($"{InitialPosition.Latitude:F6},{InitialPosition.Longitude:F6},{InitialHeading:F0}");
        }

        private static string GetDeterministicHash(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public double GetDistanceTo(GeoCoordinate geoCoordinate)
        {
            if (InitialPosition == null) throw new ArgumentException("Position null");

            return GeoUtils.GetDistanceAlongAxis(InitialPosition, geoCoordinate, InitialHeading.Value).Item1;
        }

        public STOLResult GetResult(string unit = "feet") 
        {
            if (!IsInit()) return null;

            STOLResult result = new STOLResult();
            result.Unit = unit;
            result.sessionKey = sessionKey;
            result.preset = preset;
            result.InitHash = GetInitialPosHash();
            result.User = user;
            result.planeType = planeType;
            result.planeKey = planeKey;
            result.time = (DateTime) StopTime;

            result.PatternTime = (TimeSpan) (TouchdownTime - TakeoffTime);

            result.InitialPosition = InitialPosition;
            result.InitialHeading = (double) InitialHeading;
            result.StopPosition = StopPosition;
            result.TakeoffPosition = TakeoffPosition;
            result.TouchdownPosition = TouchdownPosition;

            result.Takeoffdist = Math.Round(GetWithUnit(GetTakeoffDistance(), unit));
            result.Touchdowndist = Math.Round(GetWithUnit(GetTouchdownDistance(), unit));
            result.Stoppingdist = Math.Round(GetWithUnit(GetStoppingDistance(), unit));
            result.Landingdist = Math.Round(GetWithUnit(GetLandingDistance(), unit));
            result.GrndSpeed = Math.Round((double)TouchdownGroundSpeed);
            result.VSpeed = Math.Round((double)TouchdownVerticalSpeed);
            result.GForce = (double)TouchdownGs;

            result.maxSpin = (double)maxSpin;
            result.maxBank = (double)maxBank;
            result.minPitch = (double)minPitch;

            result.takeoffWindSpeed = takeoffWindSpeed;
            result.takeoffWindDirection = takeoffWindDirection;
            result.landingWindSpeed = landingWindSpeed;
            result.landingWindDirection = landingWindDirection;

            result.Score = result.Takeoffdist + result.Landingdist;
            if (result.Touchdowndist < 0)
            {
                result.Score = 0;
            }

            PlaneConfig planeConf = PlaneConfigsService.GetPlaneConfig(planeKey);
            result.MaxG = planeConf.MaxGForce;
            result.MaxVS = planeConf.MaxVSpeed;

            result.deviation = deviations;

            return result;

        }

        private static double GetWithUnit(double valueMeters, string unit = "feet")
        {
            switch (unit)
            {
                case "meters":
                    return valueMeters;
                case "feet":
                    return valueMeters * 3.28084;
                case "yard":
                    return valueMeters * 1.09361;
            }
            return valueMeters;
        }

        public void Reset()
        {
            TakeoffPosition = null;
            TouchdownPosition = null;
            StopPosition = null;
            TouchdownGroundSpeed = null;
            TouchdownVerticalSpeed = null;
            TouchdownPitch = null;
            StartTime = null;
            TakeoffTime = null;
            TouchdownTime = null;
            StopTime = null;
            this.deviations = new List<STOLDeviation>();
            maxSpin = null;
            minPitch = null;
            maxBank = null;
        }

        public void Retry() 
        {
            TouchdownPosition = null;
            StopPosition = null;
            TouchdownGroundSpeed = null;
            TouchdownVerticalSpeed = null;
            TouchdownPitch = null;
            TouchdownTime = null;
            StopTime = null;
        }

        public void ApplyPreset(Preset preset) 
        {
            this.preset = preset;
            InitialPosition = preset.getStart();
            InitialHeading = preset.startHeading;
            InitialPitch = 0;
            PatternAlt = preset.getPatternAltitude();
            Reset();
            Console.WriteLine($"STOL cycle initiated: {GetInitialPosHash()}\nSTART: {GeoUtils.ConvertToDMS(InitialPosition)} HDG: {Math.Round(InitialHeading.Value)}°");
        }

        public void ApplyOpenWorld(Plane plane)
        {
            this.preset = null;
            InitialPosition = plane.getPositionWithGearOffset();
            InitialHeading = plane.Heading;
            InitialPitch = 0;
            PatternAlt = ((int) Math.Ceiling(InitialPosition.Altitude * 3.28084 / 100) * 100) + 500;
            Reset();
        }

        public STOLData Copy()
        {
            throw new NotImplementedException();
            return new STOLData();
        }

        public bool hasDeviation(string key)
        {
            return deviations.FirstOrDefault((v) => v.Type == key, null) != null;
        }
    }
    public class STOLDeviation 
    {
        public STOLDeviation(string type = "", double value = 0.0, int severyty = 0) 
        {
            this.Type = type;
            this.Value = value;
            this.Severity = severyty;
        }

        public string Type;
        public double Value;
        public int Severity; // 0: Remark, 1: Waring, 2: Deviation, 3: Violation

        public string ToString() 
        {
            return this.Type + ": " + this.Value;
        }
    }

    public class STOLResult 
    {
        public Preset preset ;
        public string InitHash; 
        public TimeSpan PatternTime;
        public DateTime time;
        public string User;
        public string planeType;
        public string planeKey;
        public GeoCoordinate InitialPosition;
        public GeoCoordinate TakeoffPosition;
        public GeoCoordinate TouchdownPosition;
        public GeoCoordinate StopPosition;
        public double InitialHeading;
        public double Takeoffdist;
        public double Touchdowndist;
        public double Stoppingdist;
        public double Landingdist;
        public double minPitch;
        public double maxSpin;
        public double maxBank;
        public double GrndSpeed;
        public double VSpeed;
        public double MaxVS;
        public double Score;
        public string Unit;
        public double GForce;
        public double MaxG;
        public List<STOLDeviation> deviation;
        public string sessionKey;
        public double takeoffWindSpeed;
        public double takeoffWindDirection;
        public double landingWindSpeed;
        public double landingWindDirection;

        public string getConsoleString() 
        {
            string patternTimeStr = $"{(int)PatternTime.TotalMinutes:00}:{PatternTime.Seconds:00}";
            string scratchText = "";
            if (Touchdowndist < 0)
            {
                scratchText = " - SCRATCH!";
            }

            string resultStr = $"\r\n-----------------------------------\r\n" +
                $"Result {User} - {time}\r\n" +
                $"Plane:               {planeType}\r\n" +
                $"Takeoff Distance:    {Takeoffdist} {Unit}\r\n" +
                $"Landing Distance:    {Landingdist} {Unit}\r\n" +
                $"Stopping Distance:   {Stoppingdist} {Unit}\r\n" +
                $"Touchdown Distance:  {Touchdowndist} {Unit}{scratchText}\r\n" +
                $"Pattern Time:        {patternTimeStr} min\r\n" +
                $"TD Max Spin:         {Math.Round(maxSpin)}°\r\n" +
                $"TD Max Bank:         {Math.Round(maxBank)}°\r\n" +
                $"TD Min Pitch:        {Math.Round(minPitch)}°\r\n" +
                $"TD Grnd-Speed        {Math.Round(GrndSpeed)} kt\r\n" +
                $"TD Vert-Speed        {VSpeed:F0}/{MaxVS} f/m\r\n" +
                $"TD G-Force           {GForce:F1}/{MaxG} G\r\n" +
                $"Takeoff Wind:        {takeoffWindSpeed:F1} kt @ {takeoffWindDirection:F0}°\r\n" +
                $"Landing Wind:        {landingWindSpeed:F1} kt @ {landingWindDirection:F0}°\r\n" +
                $"-----------------------------------\r\n" +
                $"Score:               {Score}\r\n" +
                $"===================================\r\n";
            
            if (this.deviation != null && this.deviation.Count > 0) 
            {
                resultStr += $"Deviations & Remarks:               {string.Join(", ", this.deviation.Select(v => v.Type))}\r\n";
            } 
            
            return resultStr;
        }
        public string getCsvString() 
        {
            string patternTimeStr = $"{(int)PatternTime.TotalMinutes:00}:{PatternTime.Seconds:00}";
            return $"{planeType},{time},{Takeoffdist},{Landingdist},{Stoppingdist},{Touchdowndist},{patternTimeStr}, ,{GrndSpeed},{VSpeed},{Score},{Unit},{GForce}";
        }

        public static string getCSVHeader()
        {
            return "Plane,Time,Takeoff Distance,Landing Dinstance,Stopping Dinstance,Touchdown Dinstance,Pattern Time,TD Pitch,TD Grnd-Speed,TD Vert-Speed,Score,Unit,G-Force";
        }
    }
}
