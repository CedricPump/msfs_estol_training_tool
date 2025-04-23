using System;
using System.Collections.Generic;
using System.Device.Location;
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
        public string user = "";
        Preset preset = null;

        // Positions
        public GeoCoordinate InitialPosition = null;
        public GeoCoordinate TakeoffPosition = null;
        public GeoCoordinate TouchdownPosition = null;
        public GeoCoordinate StopPosition = null;

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

        public List<STOLViolation> violations = new List<STOLViolation>();



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
            result.preset = preset;
            result.InitHash = GetInitialPosHash();
            result.User = user;
            result.planeType = planeType;
            result.time = (DateTime) StopTime;

            result.PatternTime = (TimeSpan) (TouchdownTime - TakeoffTime);

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

            result.Score = result.Takeoffdist + result.Landingdist;
            if (result.Touchdowndist < 0)
            {
                result.Score = 0;
            }

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
            this.violations = new List<STOLViolation>();
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
            Reset();
            Console.WriteLine($"STOL cycle initiated: {GetInitialPosHash()}\nSTART: {GeoUtils.ConvertToDMS(InitialPosition)} HDG: {Math.Round(InitialHeading.Value)}°");
        }

        public STOLData Copy() 
        {
            throw new NotImplementedException();
            return new STOLData();
            
        }
    }

    public class STOLViolation 
    {
        public STOLViolation(string type = "", double value = 0.0) 
        {
            this.Type = type;
            this.Value = value;
        }

        public string Type;
        public double Value;

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
        public double Takeoffdist;
        public double Touchdowndist;
        public double Stoppingdist;
        public double Landingdist;
        public double minPitch;
        public double maxSpin;
        public double maxBank;
        public double GrndSpeed;
        public double VSpeed;
        public double Score;
        public string Unit;
        public double GForce;

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
                $"Max Spin:            {maxSpin}°\r\n" +
                $"Max Bank:            {maxBank}°\r\n" +
                $"Min Pitch:           {minPitch}°\r\n" +
                $"TD Grnd-Speed        {GrndSpeed} knots\r\n" +
                $"TD Vert-Speed        {VSpeed} ft/min\r\n" +
                $"TD G-Force           {GForce} G\r\n" +
                $"Start:               {InitHash}\r\n" +
                $"-----------------------------------\r\n" +
                $"Score:               {Score}\r\n" +
                $"===================================\r\n";
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
