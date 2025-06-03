
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace eSTOL_Training_Tool_Core.Core
{
    internal class Config
    {
        private const string ConfigFilePath = "config.json";


        public int IdleRefreshInterval { get; set; } = 10000;
        public int RefreshInterval { get; set; } = 100;
        public int TelemetrySendInterval { get; set; } = 3;
        public double GroundspeedThreshold { get; set; } = 0.7;
        public string ExportPath { get; set; } = "eSTOL_Training_Tool.csv";
        public string PresetsPath { get; set; } = "presets.json";
        public string CustomPresetsPath { get; set; } = "custom_presets.json";
        public string OffsetPath { get; set; } = "GearOffset.json";
        public string UserPath { get; set; } = "user.txt";
        public string Unit { get; set; } = "feet";
        public string TrikeFile { get; set; } = "Trikes.csv";
        public string[] trikesTypes { get; set; } = [];
        public bool debug { get; set; } = false;
        public bool alwaysOnTop { get; set; } = false;
        public bool isSendTelemetry { get; set; } = false;
        public bool isSendResults { get; set; } = false;
        public int uiRefreshIntervall { get; set; } = 1000;


        private static Config instance = null;

        public static Config Load()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    instance = JsonSerializer.Deserialize<Config>(json) ?? new Config();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading config, using defaults: " + ex.Message);
                    instance = new Config();
                }
            }
            else
            {
                var defaultConfig = new Config();
                defaultConfig.Save(); // Save the default config
                instance = defaultConfig;
            }

            List<string> trikesList = new List<string>();
            foreach (var line in File.ReadLines(instance.TrikeFile))
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    trikesList.Add(trimmed);
            }

            instance.trikesTypes = trikesList.ToArray();

            
            return instance;
        }

        public static Config GetInstance() 
        {
            if (instance != null) return instance;
            return Config.Load();
        }  

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            try
            {
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("unable to save config"); 
            }
        }
    }
}
