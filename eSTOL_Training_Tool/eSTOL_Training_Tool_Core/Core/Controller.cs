using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using eSTOL_Training_Tool;
using eSTOL_Training_Tool_Core.Model;
using eSTOL_Training_Tool_Core.Influx;
using System.Globalization;
using System.Linq;
using Microsoft.FlightSimulator.SimConnect;
using System.Windows.Forms;
using eSTOL_Training_Tool_Core.UI;

namespace eSTOL_Training_Tool_Core.Core
{
    enum CycleState
    {
        Park,
        Taxi,
        Takeoff,
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
        bool openWorldMode = true;
        bool presetOutputEnable = false;
        List<Preset> presets = new();
        Preset preset;
        public string user = "";
        MyInflux influx = MyInflux.GetInstance();
        FormUI? form;
        string unit;
        public bool issendResults = true;
        public bool issendTelemetry = true;
        DateTime lastTelemetrieTime = DateTime.MinValue;

        public Controller()
        {
            this.config = Config.Load();
            
            this.unit = config.Unit;
            plane = new Plane(OnPlaneEventCallback);

            // init export file
            if (!File.Exists(config.ExportPath))
            {
                // Create the file and write the header
                using (StreamWriter writer = new StreamWriter(config.ExportPath))
                {
                    writer.WriteLine(STOLResult.getCSVHeader());
                }
            }
            Console.WriteLine("exporting to " + config.ExportPath);
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
            while (!plane.isInit) { plane.Update(); System.Threading.Thread.Sleep(330); };
            while (plane.Title == null) { plane.Update(); System.Threading.Thread.Sleep(330); };

            //plane.SpawnObject("Cone",plane.Latitude,plane.Longitude,plane.Altitude);


            Console.WriteLine($"Using:\nType: \"{plane.Type}\"\nModel: \"{plane.Model}\"\nTitle: \"{plane.Title}\"\n");

            if (!File.Exists(config.UserPath))
            {
                // Disclaimer
                string disclaimer = "Disclaimer:\nThis Tool is intended for training purposes only.\nThe numbers give a quick feedback and rough estimate of your performance.They do not guarantee any accuracy.\n"
                    + "Do not challenge any competition score based on this tools' estimation alone.\nMake sure to record your flight for any necessary score validation.";
                Console.Write("\n"+disclaimer);

                MessageBox.Show(disclaimer);

                // User input
                Console.Write("You can upload your training data to database. Leave empty to ignore.\nInput eSTOL User Name: ");
                string name = Console.ReadLine();

                using (StreamWriter writer = new StreamWriter(config.UserPath))
                {
                    writer.WriteLine(name);
                    user = name;
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

            CheckForUpdate();

            // Load presets
            presets = Preset.ReadPresets(config.PresetsPath);
            if (form != null)
            {
                form.setPresets(presets.Select(p => p.title).ToArray());
            }

            openWorldMode = true;
            return;
        }

        private async void CheckForUpdate()
        {
            string? result = await VersionHelper.CheckForUpdateAsync();
            if (result != null)
            {
                MessageBox.Show($"New Version available: {result}\nhttps://github.com/CedricPump/msfs_estol_training_tool/releases");
            }
        }

        public string createPreset() 
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"Preset JSON:\r\n\r\n{{\"title\": \"YOUR TITLE\", \"start_lat\": {plane.GetTelemetrie().Position.Latitude.ToString("0.000000000000", nfi)}, \"start_long\": {plane.GetTelemetrie().Position.Longitude.ToString("0.000000000000", nfi)}, \"start_alt\": {plane.GetTelemetrie().Position.Altitude.ToString("0", nfi)}, \"start_hdg\": {plane.GetTelemetrie().Heading.ToString("0", nfi)}}}\r\n\r\ncopy and insert to presets.json";
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

        public void setUnit(string unit) 
        {
            this.unit = unit;
            if(stol != null && stol.StopPosition !=  null) 
            {
                form.setResult(stol.GetResult(this.unit).getConsoleString());
            }

        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    plane.Update();
                    if (plane.IsSimConnectConnected && !plane.SimDisabled)
                    {
                        //Console.WriteLine(cycleState);
                        Telemetrie telemetrie = plane.GetTelemetrie();
                        if (issendTelemetry && stol.user != null && stol.user != "")
                        {
                            if (DateTime.Now - lastTelemetrieTime > TimeSpan.FromSeconds(config.TelemetrySendInterval))
                            {
                                lastTelemetrieTime = DateTime.Now;
                                influx.sendTelemetry(stol.user, plane);
                            }
                        }

                        if (stol.IsInit())
                        {
                            switch (cycleState)
                            {
                                case CycleState.Unknown:
                                    if (plane.GroundSpeed < config.GroundspeedThreshold && plane.IsOnGround)
                                    {
                                        // Console.WriteLine($"Debug - Geo: ({stol.InitialPosition.Latitude} {stol.InitialPosition.Longitude} {stol.InitialPosition.Altitude}) HDG: {Math.Round((double)stol.InitialHeading)}°");
                                        setState(CycleState.Hold);
                                    }
                                    break;
                                case CycleState.Hold:
                                    {
                                        if (plane.GroundSpeed > config.GroundspeedThreshold && plane.IsOnGround)
                                        {
                                            setState(CycleState.Takeoff);
                                            stol.StartTime = DateTime.Now;
                                        }
                                        break;
                                    }
                                case CycleState.Takeoff:
                                    {
                                        if (!plane.IsOnGround)
                                        {
                                            setState(CycleState.Fly);
                                            stol.TakeoffPosition = telemetrie.Position;
                                            stol.TakeoffTime = DateTime.Now;
                                            Console.WriteLine($"Takoff recorded: {(stol.GetTakeoffDistance() * 3.28084).ToString("0")} ft");
                                            form.setResult($"Takoff recorded: {(stol.GetTakeoffDistance() * 3.28084).ToString("0")} ft");
                                        }
                                        break;
                                    }
                                case CycleState.Fly:
                                    {
                                        if (plane.IsOnGround)
                                        {
                                            // Touchdown!!!
                                            setState(CycleState.Rollout);
                                            stol.planeType = plane.Title;
                                            stol.TouchdownPosition = telemetrie.Position;
                                            stol.TouchdownTime = DateTime.Now;
                                            stol.TouchdownPitch = lastTelemetrie.pitch;
                                            stol.TouchdownGroundSpeed = lastTelemetrie.GroundSpeed;
                                            stol.TouchdownVerticalSpeed = lastTelemetrie.verticalSpeed;
                                            // nextline after clock
                                            Console.WriteLine("\nTouchdown recorded");
                                            form.setResult("Touchdown recorded");
                                        }
                                        break;
                                    }
                                case CycleState.Rollout:
                                    {
                                        if (plane.GroundSpeed < config.GroundspeedThreshold && plane.IsOnGround)
                                        {
                                            setState(CycleState.Hold);
                                            stol.StopPosition = telemetrie.Position;
                                            stol.StopTime = DateTime.Now;

                                            // End Cycle
                                            STOLResult result = stol.GetResult(unit);
                                            Console.WriteLine(result.getConsoleString());
                                            form.setResult(result.getConsoleString());
                                            form.DrawResult(result);

                                            lastStol = stol;
                                            stol.Reset();
                                            try
                                            {
                                                using (StreamWriter writer = new StreamWriter(config.ExportPath, append: true))
                                                {
                                                    writer.WriteLine(result.getCsvString());
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to export to " + config.ExportPath);
                                            }
                                            try
                                            {
                                                // send influx
                                                if (issendResults && user != null && user != "") influx.sendData(result);
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to send result");
                                            }
                                        }
                                        // Alt AGL > 5 fl to avoid bounce detection
                                        if (!plane.IsOnGround && plane.AltitudeAGL > 10)
                                        {
                                            // touch n go
                                            setState(CycleState.Fly);
                                            // stol.Retry();
                                            // stol.Reset();
                                            Console.WriteLine("Touch 'n' go recorded, try Again");
                                            form.setResult("Touch 'n' go recorded, try Again");
                                        }
                                        break;
                                    }
                            }

                            // show clock when in the air
                            if (stol.TakeoffTime != null && stol.TouchdownTime == null)
                            {
                                Console.Write($"\rtime: {(DateTime.Now - stol.TakeoffTime).Value:mm\\:ss}");
                            }
                        }
                        else
                        {

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
                        if (openWorldMode && plane.IsOnGround && plane.GroundSpeed < config.GroundspeedThreshold)
                        {
                            // initSTOL();
                        }
                        break;
                    }
                case "SMOKE_TOGGLE":
                    {
                        if (openWorldMode && plane.IsOnGround && plane.GroundSpeed < config.GroundspeedThreshold)
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
                Console.WriteLine("You selected Open World Mode.");
                Console.WriteLine("set START position and heading.");
                preset = null;
                stol.Reset();
                openWorldMode = true;
                return;
            }

            var selectedPreset = presets.Where(p => p.title == presetTitle).FirstOrDefault();

            Console.WriteLine($"You selected: {selectedPreset.title}");
            preset = selectedPreset;
            openWorldMode = false;
            stol.Reset();
            stol.ApplyPreset(preset);
        }

        public void SetStartPos()
        {
            initSTOL();
        }

        public bool IsStilInit() 
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
            Console.WriteLine($"STOL cycle initiated\nSTART: {GeoUtils.ConvertToDMS(stol.InitialPosition)} HDG: {Math.Round(stol.InitialHeading.Value)}°");

            if(presetOutputEnable)
            {
                Console.WriteLine($"Debug - Geo: ({stol.InitialPosition.Latitude} {stol.InitialPosition.Longitude} {stol.InitialPosition.Altitude}) HDG: {Math.Round((double)stol.InitialHeading)}°");
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                Console.WriteLine($"Preset JSON:\n{{\"title\": \"YOUR TITLE\", \"start_lat\": {stol.InitialPosition.Latitude.ToString("0.000000000000", nfi)}, \"start_long\": {stol.InitialPosition.Longitude.ToString("0.000000000000", nfi)}, \"start_alt\": {stol.InitialPosition.Altitude.ToString("0", nfi)}, \"start_hdg\": {stol.InitialHeading?.ToString("0", nfi)}}}");
            }
        }

        public void TeleportToReferenceLine() 
        {           
            plane.setPosition(stol.InitialPosition, stol.InitialHeading ?? 0);
        }
    }
}
