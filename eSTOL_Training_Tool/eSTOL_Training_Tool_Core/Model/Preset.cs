using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Device.Location;
using System.Text.Json;
using System.IO;

namespace eSTOL_Training_Tool_Core.Model
{
    public class Preset
    {
        [JsonProperty("title")]
        public string title = "";

        [JsonProperty("start_lat")]
        public double startLatitude = 0;

        [JsonProperty("start_long")]
        public double startLongitude = 0;

        [JsonProperty("start_alt")]
        public double startAltitude = 0;

        [JsonProperty("start_hdg")]
        public double startHeading = 0;

        public GeoCoordinate getStart()
        {
            return new GeoCoordinate(startLatitude, startLongitude, startAltitude);
        }

        public static List<Preset> ReadPresets(string filePath)
        {
            // Path to the JSON file
            filePath = "presets.json";

            // Ensure the file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return new List<Preset>();
            }

            try
            {
                // Read the JSON content from the file
                string json = File.ReadAllText(filePath);

                // Deserialize JSON into a list of Presets
                List<Preset> presets = JsonConvert.DeserializeObject<List<Preset>>(json);
                return presets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or deserializing JSON file: {ex.Message}");
                return new List<Preset>();
            }
        }
    }
}
