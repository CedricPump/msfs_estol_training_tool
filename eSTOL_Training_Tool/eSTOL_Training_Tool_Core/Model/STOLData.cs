using System;
using System.Device.Location;

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

        // Timing
        public DateTime? StartTime = null;
        public DateTime? TakeoffTime = null;
        public DateTime? TouchdownTime = null;
        public DateTime? StopTime = null;



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
            return $"{InitialPosition.Latitude:F8},{InitialPosition.Longitude:F8},{InitialHeading:F0}".GetHashCode().ToString("X");
        }

        public STOLResult GetResult() 
        {
            if (!IsInit()) return null;

            STOLResult result = new STOLResult();
            result.preset = preset;
            result.InitHash = GetInitialPosHash();
            result.User = user;
            result.planeType = planeType;
            result.time = (DateTime) StopTime;

            result.PatternTime = (TimeSpan) (TouchdownTime - TakeoffTime);

            result.Takeoffdist = Math.Round(GetTakeoffDistance() * 3.28084);
            result.Touchdowndist = Math.Round(GetTouchdownDistance() * 3.28084);
            result.Stoppingdist = Math.Round(GetStoppingDistance() * 3.28084);
            result.Landingdist = Math.Round(GetLandingDistance() * 3.28084);
            result.TdPitch = (double)(InitialPitch - TouchdownPitch);
            result.GrndSpeed = Math.Round((double)TouchdownGroundSpeed);
            result.VSpeed = Math.Round((double)TouchdownVerticalSpeed);

            result.Score = result.Takeoffdist + result.Landingdist;
            if (result.Touchdowndist < 0)
            {
                result.Score = 0;
            }

            return result;

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
        public double TdPitch;
        public double GrndSpeed;
        public double VSpeed;
        public double Score;

        public string getConsoleString() 
        {
            string patternTimeStr = $"{(int)PatternTime.TotalMinutes:00}:{PatternTime.Seconds:00}";
            string scratchText = "";
            if (Touchdowndist < 0)
            {
                scratchText = " - SCRATCH!";
            }

            string resultStr = $"\n-----------------------------------\n" +
                $"Result {User} - {time}\n" +
                $"Plane:               {planeType}\n" +
                $"Takeoff Dinstance:   {Takeoffdist} ft\n" +
                $"Landing Dinstance:   {Landingdist} ft\n" +
                $"Stopping Dinstance:  {Stoppingdist} ft\n" +
                $"Touchdown Dinstance: {Touchdowndist} ft{scratchText}\n" +
                $"Pattern Time:        {patternTimeStr} min\n" +
                $"TD Pitch:            {TdPitch}°\n" +
                $"TD Grnd-Speed        {GrndSpeed} knots\n" +
                $"TD Vert-Speed        {VSpeed} ft/min\n" +
                $"Start:               {InitHash}\n" +
                $"-----------------------------------\n" +
                $"Score:               {Score}\n" +
                $"===================================\n";
            return resultStr;
        }
        public string getCsvString() 
        {
            string patternTimeStr = $"{(int)PatternTime.TotalMinutes:00}:{PatternTime.Seconds:00}";
            return $"{planeType},{time},{Takeoffdist},{Landingdist},{Stoppingdist},{Touchdowndist},{patternTimeStr},{TdPitch},{GrndSpeed},{VSpeed},{Score}";
        }

        public static string getCSVHeader()
        {
            return "Plane,Time,Takeoff Distance,Landing Dinstance,Stopping Dinstance,Touchdown Dinstance,Pattern Time,TD Pitch,TD Grnd-Speed,TD Vert-Speed,Score";
        }

        public string GetInfluxLineProtocol() 
        {
            string measurement = "stol_results";
            string tags = $"User={User},planeType={planeType}";
            string fields = $"Takeoffdist={Takeoffdist}," +
                            $"Touchdowndist={Touchdowndist}," +
                            $"Stoppingdist={Stoppingdist}," +
                            $"Landingdist={Landingdist}," +
                            $"TdPitch={TdPitch}," +
                            $"GrndSpeed={GrndSpeed}," +
                            $"VSpeed={VSpeed}," +
                            $"Score={Score}," +
                            $"StartHash={InitHash}" +
                            $"PatternTime={PatternTime.TotalSeconds}";
                            
            long timestamp = new DateTimeOffset(time).ToUnixTimeMilliseconds() * 1_000_000; // Convert to nanoseconds.

            return $"{measurement},{tags} {fields} {timestamp}";
        }
    }
}
