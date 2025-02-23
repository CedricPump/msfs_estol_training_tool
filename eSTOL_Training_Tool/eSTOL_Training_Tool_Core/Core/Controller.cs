using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using eSTOL_Training_Tool;
using eSTOL_Training_Tool_Core.Model;
using eSTOL_Training_Tool_Core.Influx;
using System.Globalization;
using NodaTime;
using System.Linq;
using Microsoft.FlightSimulator.SimConnect;
using System.Windows.Forms;
using eSTOL_Training_Tool_Core.UI;
using System.Xml.Linq;

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
        int idlerefreshIntervall = 10000;
        int refreshIntervall = 250;
        CycleState cycleState = CycleState.Unknown;
        STOLData stol = new STOLData();
        double groundspeedThreshold = 0.7;
        Telemetrie lastTelemetrie;
        string exportPath = "eSTOL_Training_Tool.csv";
        bool openWorldMode = true;
        bool presetOutputEnable = false;
        List<Preset> presets = new();
        Preset preset;
        public string user = "";
        const string presetsPath = "presets.json";
        const string offsetPath = "GearOffset.json";
        string userPath = "user.txt";
        MyInflux influx = MyInflux.GetInstance();
        FormUI? form;

        public Controller()
        {
            plane = new Plane(OnPlaneEventCallback);

            // init export file
            if (!File.Exists(exportPath))
            {
                // Create the file and write the header
                using (StreamWriter writer = new StreamWriter(exportPath))
                {
                    writer.WriteLine(STOLResult.getCSVHeader());
                }
            }
            Console.WriteLine("exporting to " + exportPath);
        }

        public void SetUI(FormUI form) 
        {
            this.form = form;
            this.form.setPresets(presets.Select(p => p.title).ToArray());
        }

        public void Init()
        {
            GearOffset.LoadOffsetDict(offsetPath);

            // Update once to trigger connect to sim
            plane.Update();
            while (!plane.isInit) { plane.Update(); System.Threading.Thread.Sleep(330); };
            while (plane.Title == null) { plane.Update(); System.Threading.Thread.Sleep(330); };

            //plane.SpawnObject("Cone",plane.Latitude,plane.Longitude,plane.Altitude);


            Console.WriteLine($"Using:\nType: \"{plane.Type}\"\nModel: \"{plane.Model}\"\nTitle: \"{plane.Title}\"\n");

            if (!File.Exists(userPath))
            {
                // Disclaimer
                string disclaimer = "Disclaimer:\nThis Tool is intended for training purposes only.\nThe numbers give a quick feedback and rough estimate of your performance.They do not guarantee any accuracy.\n"
                    + "Do not challenge any competition score based on this tools' estimation alone.\nMake sure to record your flight for any necessary score validation.";
                Console.Write("\n"+disclaimer);

                MessageBox.Show(disclaimer);

                // User input
                Console.Write("You can upload your training data to database. Leave empty to ignore.\nInput eSTOL User Name: ");
                string name = Console.ReadLine();

                using (StreamWriter writer = new StreamWriter(userPath))
                {
                    writer.WriteLine(name);
                    user = name;
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(userPath))
                {
                    user = reader.ReadLine(); // Read the username
                }
            }
            stol.user = user;

            // Load presets
            presets = Preset.ReadPresets(presetsPath);
            if (form != null)
            {
                form.setPresets(presets.Select(p => p.title).ToArray());
            }

            openWorldMode = true;
            return;

            // Build the selection prompt
            string prompt =
                "Select a eSTOL field preset or \"Open World\" mode (default):\n\n" +
                "  [ 0] Open World Mode (set custom start with parking brake)\n";

            for (int i = 0; i < presets.Count; i++)
            {
                prompt += $"  [{i + 1,2}] {presets[i].title}\n";
            }

            Console.WriteLine(prompt);
            Console.Write("Enter your selection (0 for Open World, or preset number): ");

            // Read input from the user
            // string input = Console.ReadLine();
            //string input = ReadLineWithTimeout(30000);
            // if (input == null || input == "") input = "0";
            string input = "0";

            // Parse the input and handle selection
            if (int.TryParse(input, out int selection))
            {
                if (selection == 0)
                {
                    Console.WriteLine("You selected Open World Mode.");
                    Console.WriteLine("toggle parking break to inititate START position and heading.");
                    openWorldMode = true;
                }
                else if (selection >= 1 && selection <= presets.Count)
                {
                    Preset selectedPreset = presets[selection - 1];
                    Console.WriteLine($"You selected: {selectedPreset.title}");
                    preset = selectedPreset;
                    openWorldMode = false;

                    stol.ApplyPreset(preset);

                    // ask for teleport
                    Console.Write($"Do you want to teleport to reference line? [y/(n)]: ");

                    string? userInput = ReadLineWithTimeout(5000);
                    if (userInput != null && userInput.ToLower() == "y")
                    {
                        TeleportToReferenceLine();
                    }
                }
                else if (selection < 0)
                {
                    Console.WriteLine("Preset creation Mode.");
                    Console.WriteLine("toggle parking break to inititate START position and heading.");
                    openWorldMode = true;
                    
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    Init(); // Restart selection
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                Init(); // Restart selection
            }


        }

        public string createPreset() 
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"Preset JSON:\r\n\r\n{{\"title\": \"YOUR TITLE\", \"start_lat\": {plane.GetTelemetrie().Position.Latitude.ToString("0.000000000000", nfi)}, \"start_long\": {plane.GetTelemetrie().Position.Longitude.ToString("0.000000000000", nfi)}, \"start_alt\": {plane.GetTelemetrie().Position.Altitude.ToString("0", nfi)}, \"start_hdg\": {plane.GetTelemetrie().Heading.ToString("0", nfi)}}}\r\n\r\ncopy and insert to presets.json";
        }

        public void SetUser(string username)
        {
            using (StreamWriter writer = new StreamWriter(userPath))
            {
                writer.WriteLine(username);
                user = username;
            }
            stol.user = username;
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
                        //Console.WriteLine(GeoUtils.GetDistanceAlongAxis(telemetrie.Position, stol.InitialPosition, stol.InitialHeading.Value));

                        if (stol.IsInit())
                        {
                            // var (x, y) = GeoUtils.GetOffsetXYByHeading(stol.InitialPosition, telemetrie.Position, (double)stol.InitialHeading);
                            // var dist = stol.InitialPosition.GetDistanceTo(telemetrie.Position);
                            // Console.WriteLine($"dist: {Math.Round(dist* 3.28084)} offset: ({Math.Round(x)},{Math.Round(y)})");

                            switch (cycleState)
                            {
                                case CycleState.Unknown:
                                    if (plane.GroundSpeed < groundspeedThreshold && plane.IsOnGround)
                                    {
                                        // Console.WriteLine($"Debug - Geo: ({stol.InitialPosition.Latitude} {stol.InitialPosition.Longitude} {stol.InitialPosition.Altitude}) HDG: {Math.Round((double)stol.InitialHeading)}°");
                                        setState(CycleState.Hold);
                                    }
                                    break;
                                case CycleState.Hold:
                                    {
                                        if (plane.GroundSpeed > groundspeedThreshold && plane.IsOnGround)
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
                                        if (plane.GroundSpeed < groundspeedThreshold && plane.IsOnGround)
                                        {
                                            setState(CycleState.Hold);
                                            stol.StopPosition = telemetrie.Position;
                                            stol.StopTime = DateTime.Now;

                                            // End Cycle
                                            STOLResult result = stol.GetResult();
                                            Console.WriteLine(result.getConsoleString());
                                            form.setResult(result.getConsoleString());

                                            stol.Reset();
                                            try
                                            {
                                                using (StreamWriter writer = new StreamWriter(exportPath, append: true))
                                                {
                                                    writer.WriteLine(result.getCsvString());
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Unable to export to " + exportPath);
                                            }
                                            try
                                            {
                                                // send influx
                                                if (user != "") influx.sendData(result);
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
                int intervall = plane.IsSimConnectConnected ? refreshIntervall : idlerefreshIntervall;
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
                        if (openWorldMode && plane.IsOnGround && plane.GroundSpeed < groundspeedThreshold)
                        {
                            // initSTOL();
                        }
                        break;
                    }
                case "SMOKE_TOGGLE":
                    {
                        if (openWorldMode && plane.IsOnGround && plane.GroundSpeed < groundspeedThreshold)
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

        static string ReadLineWithTimeout(int timeoutMilliseconds)
        {
            Task<string> inputTask = Task.Run(() => Console.ReadLine());

            if (Task.WaitAny(new[] { inputTask }, timeoutMilliseconds) == 0)
            {
                return inputTask.Result; // Task completed within timeout
            }

            return null; // Timeout reached
        }
    }
}
