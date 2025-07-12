using System;
using System.Collections.Generic;
using System.IO;
using eSTOL_Training_Tool;
using eSTOL_Training_Tool_Core.Model;
using eSTOL_Training_Tool_Core.Influx;
using System.Globalization;
using System.Linq;
using Microsoft.FlightSimulator.SimConnect;
using System.Windows.Forms;
using eSTOL_Training_Tool_Core.UI;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Device.Location;
using NodaTime;
using eSTOL_Training_Tool_Core.GPX;

namespace eSTOL_Training_Tool_Core.Core
{
    enum CycleState
    {
        Park,
        Taxi,
        Takeoff,
        Climbout,
        Fly,
        Land,
        Rollout,
        Hold,
        Unknown
    }

    public class Controller
    {
        Plane plane;
        Config config;
        CycleState cycleState = CycleState.Unknown;
        STOLData stol = new STOLData();
        STOLData lastStol = new STOLData();
        Telemetrie lastTelemetrie;
        GPXRecorder GPXRecorder = new GPXRecorder();
        bool openWorldMode = true;
        List<Preset> presets = new();
        Preset preset;
        public string user = "";
        MyInflux influx = MyInflux.GetInstance();
        FormUI? form;
        string unit;
        DateTime lastTelemetrieTime = DateTime.MinValue;
        DateTime lastUIResfresh = DateTime.MinValue;
        double AGLonGroundThreshold = 0.3; // ft
        double fieldLength = 600; // ft
        bool debug = false;
        double flagsAngleTreshold = 1; // deg

        public Controller()
        {
            this.config = Config.Load();
            
            this.unit = config.Unit;
            plane = new Plane(OnPlaneEventCallback);

            var exportDir = Path.GetDirectoryName(config.ResultsExportPath);
            // fix wrong export file from 1.3.1
            if (File.Exists(exportDir))
            {
                File.Delete(exportDir);
            }
            // add export dir#
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            // init export file
            if (!File.Exists(config.ResultsExportPath))
            {
                // Create the file and write the header
                using (StreamWriter writer = new StreamWriter(config.ResultsExportPath))
                {
                    writer.WriteLine(STOLResult.getCSVHeader());
                }
            }
            if (config.debug) Console.WriteLine("exporting to " + config.ExportPath);
        }

        public void SetUI(FormUI form) 
        {
            this.form = form;
            this.form.setPresets(presets.Select(p => p.title).ToArray());
        }

        public void Init()
        {
            GearOffset.LoadOffsetDict(this.config.OffsetPath);

            // Update once to trigger connect to sim
            plane.Update();

            //plane.SpawnObject("Cone",plane.Latitude,plane.Longitude,plane.Altitude);

            LoadUser();

            CheckForUpdate();

            // Load presets
            reloadPreset();

            openWorldMode = true;
            return;
        }

        private void LoadUser()
        {
            if (!File.Exists(config.UserPath))
            {
                // Disclaimer
                string disclaimer = "Disclaimer:\n"
                    + "This tool is a work in progress and is primarily intended for training purposes.\n"
                    + "It may contain bugs or incomplete features, and some values may not reflect perfect accuracy.\n"
                    + "Final competition results are determined solely by the official judge using all tools of their discretion.\n"
                    + "Please make sure to record your flight for any required score verification or challenges.\n\n";
                Console.Write("\n" + disclaimer);

                MessageBox.Show(disclaimer);

                using (var userForm = new FormFirstUser())
                {
                    if (userForm.ShowDialog() == DialogResult.OK)
                    {
                        string name = userForm.Username?.Trim() ?? "";

                        using (StreamWriter writer = new StreamWriter(config.UserPath))
                        {
                            writer.WriteLine(name);
                            user = name;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No username entered. Upload will be skipped.", "Info");
                        user = "";
                    }
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(config.UserPath))
                {
                    user = reader.ReadLine(); // Read the username
                }
            }
            stol.user = user;
        }

        private async void CheckForUpdate()
        {
            string? result = await VersionHelper.CheckForUpdateAsync();
            if (result != null)
            {
                using var dialog = new UpdateDialog(result);
                dialog.ShowDialog();

                if (dialog.shouldUpdate)
                {
                    await PerformUpdate(result);
                }
            }
        }

        private async Task PerformUpdate(string version)
        {
            string zipFileName = $"eSTOL_Training_Tool_portable_{version.Replace(".", "")}.zip";
            string zipUrl = $"https://github.com/CedricPump/msfs_estol_training_tool/releases/download/{version}/{zipFileName}";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "update.zip");
            string bootstrapperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.ps1");

            using HttpClient client = new HttpClient();
            using var stream = await client.GetStreamAsync(zipUrl);
            using var fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fs);

            Console.WriteLine($"Saved update to: {tempZipPath}");

            string arguments = $"-ExecutionPolicy Bypass -File \"{bootstrapperPath}\" \"{tempZipPath}\" \"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)}\"";

            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = arguments,
                UseShellExecute = false
            });



            Application.Exit();
        }


        public string createPreset() 
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"Preset JSON:\r\n\r\n{{\"title\": \"YOUR TITLE\", \"start_lat\": {plane.GetTelemetrie().Position.Latitude.ToString("0.000000", nfi)}, \"start_long\": {plane.GetTelemetrie().Position.Longitude.ToString("0.000000", nfi)}, \"start_alt\": {plane.GetTelemetrie().Position.Altitude.ToString("0", nfi)}, \"start_hdg\": {plane.GetTelemetrie().Heading.ToString("0", nfi)}}}\r\n\r\ncopy and insert to presets.json";
        }

        public void SetUser(string username)
        {
            using (StreamWriter writer = new StreamWriter(config.UserPath))
            {
                writer.WriteLine(username);
                user = username;
            }
            stol.user = username;
        }

        public void SetSession(string sessionKey)
        {
            stol.sessionKey = sessionKey;
        }

        public void setUnit(string unit) 
        {
            this.unit = unit;
            if(stol != null && stol.StopPosition !=  null) 
            {
                form.setResult(stol.GetResult(this.unit).getConsoleString());
            }

        }

        public void ReinitPlaneType() 
        {
            if(this.stol != null) 
            {
                this.stol.planeType = plane.Title;
                this.form.setResult($"Plane Changed: {this.stol.planeType}");
            }
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    plane.Update();
                    if(!plane.isInit) { 
                        plane.Update();
                        System.Threading.Thread.Sleep(330);
                        continue;
                    };

                    if (plane.Title == null)
                    {
                        plane.Update();
                        System.Threading.Thread.Sleep(330);
                        continue;
                    };

                    if(this.stol.planeType != "" && plane.Title != this.stol.planeType) 
                    {
                        ReinitPlaneType();
                    }

                    if (plane.IsSimConnectConnected && !plane.SimDisabled)
                    {
                        Telemetrie telemetrie = plane.GetTelemetrie();
                        // send telemetry and write debug
                        if (config.isSendTelemetry && stol.user != null && stol.user != "")
                        {
                            if (DateTime.Now - lastTelemetrieTime > TimeSpan.FromSeconds(config.TelemetrySendInterval))
                            {
                                lastTelemetrieTime = DateTime.Now;
                                try
                                {
                                    influx.sendTelemetry(stol.user, plane, stol);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Unable to send telemetry - check update");
                                }

                                if (config.debug && stol.IsInit()) Console.WriteLine($"lat distance to line: {stol.GetDistanceTo(telemetrie.Position) * 3.28084} AGL: {telemetrie.AltitudeAGL}");
                                if (config.debug && stol.IsInit()) Console.WriteLine($"Gear on Gorund - Main: {( plane.MainGearOnGround() ? 1 : 0 )} N/T: {(plane.TailNoseGearOnGround() ? 1 : 0)}");
                            }
                        }

                        if (stol.IsInit() && config.debug)
                        {
                            if (DateTime.Now - lastUIResfresh > TimeSpan.FromSeconds(config.uiRefreshIntervall))
                            {
                                this.form.setPlanePos(stol.InitialPosition, (double)stol.InitialHeading, telemetrie.Position);
                            }
                        }

                        if (plane.isInit && form != null)
                        {
                            if (DateTime.Now - lastUIResfresh > TimeSpan.FromMilliseconds(config.uiRefreshIntervall))
                            {
                                form.setWind(plane.getRelDir(), plane.getWindTotal());
                                lastUIResfresh = DateTime.Now;

                                // alingnment help
                                if(stol.IsInit() && (cycleState == CycleState.Takeoff || cycleState == CycleState.Hold)) 
                                {
                                    double linedist = stol.GetDistanceTo(telemetrie.Position) * 3.28084;
                                    if (linedist >= -50 && linedist <= 1)
                                    {
                                        this.form.setResult($"Align: {Math.Round(stol.GetDistanceTo(telemetrie.Position) * 3.28084) + 3.93701} ft to Line");
                                        this.form.setAligned("Aligned", System.Drawing.Color.White);
                                    }
                                }
                            }

                            if( stol.IsInit() && plane.isInit)
                            {
                                if(plane.WingtipOnGround()) 
                                {

                                    if (!stol.hasViolation("WingStrike"))
                                    {
                                        stol.violations.Add(new STOLViolation("WingStrike", 1));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol.sessionKey, plane, "WINGSTRIKE", "true");
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Unable to send event - check update");
                                        }
                                    }
                                }
                            }

                            switch (cycleState)
                            {
                                case CycleState.Unknown:
                                    if (plane.GroundSpeed < config.GroundspeedThreshold && plane.MainGearOnGround())
                                    {
                                        setState(CycleState.Hold);
                                    }
                                    break;
                                case CycleState.Hold:
                                    {
                                        if (IsStolInit())
                                        {
                                            (double yOffset, double xOffset) = GeoUtils.GetDistanceAlongAxis(stol.InitialPosition, plane.getPositionWithGearOffset(), (double)stol.InitialHeading);
                                            double spin = GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                            (double angleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);

                                            if(debug) Console.WriteLine($"angleL: {angleL:F1}°  angleR: {angleR:F1}° spin: {spin}°");

                                            if (!(spin > angleR + flagsAngleTreshold || spin < angleL - flagsAngleTreshold) && yOffset > -1.2 && yOffset < 0 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned("aligned", System.Drawing.Color.LightGreen);
                                            }
                                            else if (Math.Abs(spin) < 45 && yOffset > -1.2 && yOffset < 1 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned("aligned (bad heading)", System.Drawing.Color.LightGreen);
                                            }
                                            else if (Math.Abs(spin) < 90 && yOffset > -180 && yOffset < 1 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned("on lineup", System.Drawing.Color.LightYellow);
                                            }
                                            else
                                            {
                                                this.form.setAligned("NOT ALIGNED", System.Drawing.Color.IndianRed);
                                            }
                                        }



                                        // on start roll -> State Takeoff
                                        if (plane.GroundSpeed > config.GroundspeedThreshold && plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Takeoff);
                                            stol.StartTime = DateTime.Now;
                                            this.form.StartStopWatch();

                                            if (config.enableGPXRecodering) 
                                            {
                                                GPXRecorder.Reset();
                                            }
                                        }

                                        // on (vertical) Takeoff -> State Takeoff
                                        if (!plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Climbout);
                                            stol.StartTime = DateTime.Now;
                                            this.form.StartStopWatch();

                                            if (config.enableGPXRecodering)
                                            {
                                                GPXRecorder.Reset();
                                            }
                                        }
                                        break;
                                    }
                                case CycleState.Takeoff:
                                    {
                                        this.form.setAligned("", System.Drawing.SystemColors.Control);
                                        (double andleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);

                                        // on Takeoff -> State Climbout
                                        if (!plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Climbout);
                                            stol.TakeoffPosition = telemetrie.Position;
                                            stol.TakeoffTime = DateTime.Now;
                                            if (config.debug) Console.WriteLine($"Takoff recorded: {(stol.GetTakeoffDistance() * 3.28084).ToString("0")} ft");
                                            form.setResult($"Takoff recorded: {(stol.GetTakeoffDistance() * 3.28084).ToString("0")} ft");
                                            if (config.debug && stol.IsInit()) Console.WriteLine("TO Pos: " + stol.TakeoffPosition);

                                            try
                                            {
                                                // send event
                                                if (config.isSendResults) influx.sendEvent(user, stol.sessionKey, plane, "TAKEOFF", (stol.GetTakeoffDistance() * 3.28084).ToString("0"));
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to send event - check update");
                                            }
                                        }

                                        // stopping -> State Hold
                                        if (plane.GroundSpeed < config.GroundspeedThreshold && plane.MainGearOnGround())
                                        {
                                            // parking or alignment
                                            setState(CycleState.Hold);
                                            this.form.StopStopWatch();
                                        }

                                        break;
                                    }
                                case CycleState.Climbout:
                                    {
                                        // field length exceeded -> State Fly
                                        if(stol.GetDistanceTo(telemetrie.Position) * 3.28084 > fieldLength) 
                                        {
                                            setState(CycleState.Fly);
                                            break;
                                        }

                                        // touchdown -> State Takeoff
                                        if (plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Takeoff);
                                            break;
                                        }
                                        break;
                                    }
                                case CycleState.Fly:
                                    {
                                        // Taildragging Debug
                                        if (plane.MainGearOnGround() && !plane.TailNoseGearOnGround()) Console.WriteLine("Dragging Tial, hope you are a Taildragger: " + plane.IsTaildragger);

                                        // Touchdown -> State Rollout
                                        if (plane.MainGearOnGround())
                                        {
                                            // Touchdown!!!
                                            setState(CycleState.Rollout);
                                            stol.planeType = plane.Title;
                                            stol.TouchdownPosition = telemetrie.Position;
                                            stol.TouchdownTime = DateTime.Now;
                                            stol.TouchdownPitch = lastTelemetrie.pitch;
                                            stol.TouchdownGs = telemetrie.gForce;
                                            stol.TouchdownGroundSpeed = lastTelemetrie.GroundSpeed;
                                            stol.TouchdownVerticalSpeed = lastTelemetrie.verticalSpeed;
                                            double spin = GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                            stol.maxSpin = Math.Abs(spin);
                                            stol.maxBank = Math.Abs(telemetrie.bank);
                                            double pitch = (double)(stol.InitialPitch - telemetrie.pitch);
                                            stol.minPitch = pitch;

                                            if (config.debug && stol.IsInit()) Console.WriteLine($"max spin: {stol.maxSpin}° max bank: {stol.maxBank}° max pitch: {stol.minPitch}°");

                                            // nextline after clock
                                            if (config.debug) Console.WriteLine("\nTouchdown recorded");
                                            if (config.debug && stol.IsInit()) Console.WriteLine("TD Pos: " + stol.TouchdownPosition);
                                            form.setResult("Touchdown recorded");

                                            (double angleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);
                                            if (config.debug) Console.WriteLine($"spin: {spin:F1}°");
                                            if (spin > angleR + flagsAngleTreshold || spin < angleL - flagsAngleTreshold)
                                            {
                                                stol.violations.Add(new STOLViolation("ExcessiveTouchdownSpin", spin));
                                            }

                                            if (config.isSendResults && stol.hasViolation("ExcessiveTouchdownSpin"))
                                            {
                                                influx.sendEvent(user, stol.sessionKey, plane, "EXCESSIVE_TOUCHDOWN_SPIN", ((spin).ToString("0.0")));
                                            }

                                            double touchdowndist = stol.GetTouchdownDistance();
                                            if (touchdowndist <= 0)
                                            {
                                                stol.violations.Add(new STOLViolation("TouchdownLineViolation", touchdowndist));
                                            }

                                            if (stol.TouchdownGs > 2.0)
                                            {
                                                stol.violations.Add(new STOLViolation("ExcessiveGs", (double)stol.TouchdownGs));
                                            }

                                            if (stol.TouchdownVerticalSpeed < -500.0)
                                            {
                                                stol.violations.Add(new STOLViolation("ExcessiveVerticalSpeed", (double)stol.TouchdownGs));
                                            }



                                            try
                                            {
                                                // send event
                                                if (config.isSendResults)
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "TOUCHDOWN", (stol.GetTouchdownDistance() * 3.28084).ToString("0"));
                                                }
                                                if (config.isSendResults && stol.hasViolation("TouchdownLineViolation"))
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "SCRATCH", (stol.GetTouchdownDistance() * 3.28084).ToString("0"));
                                                }
                                                if (config.isSendResults && stol.hasViolation("ExcessiveGs"))
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "EXCESSIVE_G", (((double)stol.TouchdownGs).ToString("0.0")));
                                                }
                                                if (config.isSendResults && stol.hasViolation("ExcessiveVerticalSpeed"))
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "EXCESSIVE_VSPEED", (((double)stol.TouchdownVerticalSpeed).ToString("0")));
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine("Unable to send event - check update: " + e.Message);
                                            }

                                            
                                        }
                                        break;
                                    }
                                case CycleState.Rollout:
                                    {
                                        // record attitude on rollout
                                        // double spin = GeoUtils.GetMinDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                        double spin = GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                        stol.maxSpin = Math.Max((double) stol.maxSpin, Math.Abs(spin));
                                        stol.maxBank = Math.Max((double) stol.maxBank, Math.Abs(telemetrie.bank));
                                        double pitch = (double)(stol.InitialPitch - telemetrie.pitch);
                                        stol.minPitch = Math.Min((double)stol.minPitch, pitch);

                                        if (config.debug && stol.IsInit())
                                        { Console.WriteLine($"max spin: {stol.maxSpin}° max bank: {stol.maxBank}° max pitch: {stol.minPitch}°"); }
                                        (double angleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);
                                        if (config.debug) Console.WriteLine($"spin: {spin:F1}°");

                                        // stopping -> Hold
                                        if (plane.GroundSpeed < config.GroundspeedThreshold && plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Hold);
                                            stol.StopPosition = telemetrie.Position;
                                            stol.StopTime = DateTime.Now;
                                            this.form.StopStopWatch();

                                            if (spin > angleR + flagsAngleTreshold || spin < angleL - flagsAngleTreshold)
                                            {
                                                stol.violations.Add(new STOLViolation("ExcessiveStopSpin", (double)spin));
                                            }

                                            if (Math.Abs((double)stol.maxSpin) > 45.0)
                                            {
                                                stol.violations.Add(new STOLViolation("ExcessiveMaxSpin", (double)stol.maxSpin));
                                            }

                                            // End Cycle
                                            STOLResult result = stol.GetResult(unit);
                                            Console.WriteLine(result.getConsoleString());
                                            form.setResult(result.getConsoleString());
                                            form.DrawResult(result);

                                            try
                                            {
                                                // send event
                                                if (config.isSendResults)
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "STOP", (stol.GetLandingDistance() * 3.28084).ToString("0"));
                                                }

                                                if (config.isSendResults && stol.hasViolation("ExcessiveStopSpin"))
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "EXCESSIVE_STOP_SPIN", spin.ToString("0.0"));
                                                }

                                                if (config.isSendResults && stol.hasViolation("ExcessiveMaxSpin"))
                                                {
                                                    influx.sendEvent(user, stol.sessionKey, plane, "EXCESSIVE_MAX_SPIN", (((double)stol.maxSpin).ToString("0.0")));
                                                }

                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to send event - check update");
                                            }


                                            lastStol = stol;
                                            stol.Reset();
                                            try
                                            {
                                                var dir = Path.GetDirectoryName(config.ResultsExportPath);
                                                Directory.CreateDirectory(dir);
                                                using (StreamWriter writer = new StreamWriter(config.ResultsExportPath, append: true))
                                                {
                                                    writer.WriteLine(result.getCsvString());
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to export to " + config.ResultsExportPath);
                                            }
                                            try
                                            {
                                                // send influx
                                                if (config.isSendResults && user != null && user != "") influx.sendData(result);
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to send result - check update");
                                            }

                                            // save gpx
                                            if (config.enableGPXRecodering)
                                            {
                                                GPXRecorder.Append(telemetrie);
                                                GPXRecorder.Save(stol.user.Trim(), stol.planeType);
                                            }
                                        }
                                        // Alt AGL > 5 ft to avoid bounce detection
                                        if (!plane.MainGearOnGround() && plane.AltitudeAGL > 10)
                                        {
                                            // touch n go
                                            setState(CycleState.Fly);
                                            Console.WriteLine("Touch 'n' go recorded, try Again");
                                            form.setResult("Touch 'n' go recorded, try Again");
                                            if (config.isSendResults) influx.sendEvent(user, stol.sessionKey, plane, "TOUCH_N_GO", "true");
                                        }
                                        break;
                                    }
                            }

                            if (config.enableGPXRecodering && cycleState != CycleState.Hold && cycleState != CycleState.Unknown)
                            {
                                GPXRecorder.Append(telemetrie);
                            }
                        }
                        else
                        {
                            // wait until stol is init
                        }
                        lastTelemetrie = telemetrie;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // sllep until next interval
                int intervall = plane.IsSimConnectConnected ? config.RefreshInterval : config.IdleRefreshInterval;
                System.Threading.Thread.Sleep(intervall);
            };
        }

        private (double, double) GetFlagAngles(GeoCoordinate origin, double initHeading, Plane plane)
        {
            (double yOffset, double xOffset) = GeoUtils.GetDistanceAlongAxis(origin, plane.getPositionWithGearOffset(), initHeading);

            if (config.debug) Console.WriteLine($"x: {xOffset:F1} m  y: {yOffset:F1} m");

            double adjacent = 182.88 - yOffset;
            double halfSpan = 42.672 / 2;

            double oppositeLeft = halfSpan + xOffset;
            double oppositeRight = halfSpan - xOffset;

            if (config.debug) Console.WriteLine($"adjacent: {adjacent:F1} m  opposites: L={oppositeLeft:F1} m  R={oppositeRight:F1} m");

            double angleLeft = Math.Atan(oppositeLeft / adjacent) * (180.0 / Math.PI);
            double angleRight = Math.Atan(oppositeRight / adjacent) * (180.0 / Math.PI);

            if (config.debug) Console.WriteLine($"angleL: {angleLeft:F1}°  angleR: {angleRight:F1}°");

            return (-angleLeft, angleRight);
        }



        private void setState(CycleState state)
        {
            var old = cycleState;
            cycleState = state;
            if (form != null) 
            {
                form.setState(state.ToString());
            } 
        }

        public void OnPlaneEventCallback(PlaneEvent evt)
        {
            // Console.WriteLine(evt.Event);
            switch (evt.Event)
            {
                case "PARKING_BRAKES":
                    {
                        if (openWorldMode && plane.MainGearOnGround() && plane.GroundSpeed < config.GroundspeedThreshold)
                        {
                            // initSTOL();
                        }
                        break;
                    }
                case "SMOKE_TOGGLE":
                    {
                        if (openWorldMode && plane.MainGearOnGround() && plane.GroundSpeed < config.GroundspeedThreshold)
                        {
                            // initSTOL();
                        }
                        break;
                    }
            }
        }

        public void SetPreset(string presetTitle) 
        {
            if (presetTitle == "Open World") 
            {
                if (config.debug) Console.WriteLine("You selected Open World Mode.");
                if (config.debug) Console.WriteLine("set START position and heading.");
                preset = null;
                stol.Reset();
                openWorldMode = true;
                return;
            }

            var selectedPreset = presets.Where(p => p.title == presetTitle).FirstOrDefault();

            if (config.debug) Console.WriteLine($"You selected: {selectedPreset.title}");
            preset = selectedPreset;
            openWorldMode = false;
            stol.Reset();
            stol.ApplyPreset(preset);
            if (config.debug) Console.WriteLine($"Using:\nType: \"{plane.Type}\"\nModel: \"{plane.Model}\"\nTitle: \"{plane.Title}\"\n");
        }

        public void SetStartPos()
        {
            initSTOL();
        }

        public bool IsSimConnected()
        {
            return this.plane.isInit;
        }

        public bool IsStolInit() 
        {
            return stol.IsInit();
        }

        private void initSTOL()
        {
            // set STOL initial Values
            stol.planeType = plane.Title;
            stol.InitialHeading = plane.Heading;
            stol.InitialPitch = plane.pitch;
            stol.InitialPosition = plane.GetTelemetrie().Position;
            setState(CycleState.Hold);
            if (config.debug) Console.WriteLine($"STOL cycle initiated\nSTART: {GeoUtils.ConvertToDMS(stol.InitialPosition)} HDG: {Math.Round(stol.InitialHeading.Value)}°");
            if (config.debug) Console.WriteLine($"Using:\nType: \"{plane.Type}\"\nModel: \"{plane.Model}\"\nTitle: \"{plane.Title}\"\n");
        }

        public void TeleportToReferenceLine() 
        {           
            plane.setPosition(stol.InitialPosition, stol.InitialHeading ?? 0);
        }

        public void reloadPreset()
        {
            presets = Preset.ReadPresets(config.PresetsPath, config.CustomPresetsPath);
            if (form != null)
            {
                form.setPresets(presets.Select(p => p.title).ToArray());
            }
            GearOffset.LoadOffsetDict(this.config.OffsetPath);
        }
    }
}
