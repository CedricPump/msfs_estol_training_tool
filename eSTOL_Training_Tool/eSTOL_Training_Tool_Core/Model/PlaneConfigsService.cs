using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace eSTOL_Training_Tool_Core.Model
{
    public static class PlaneConfigsService
    {
        private static Dictionary<string, PlaneConfig> planeConfigByKey = new();
        private static List<PlaneConfig> planeConfigs = new();

        public static void LoadPlaneConfigs(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                planeConfigs = JsonSerializer.Deserialize<List<PlaneConfig>>(json) ?? new();

                planeConfigByKey.Clear();
                foreach (PlaneConfig config in planeConfigs)
                {
                    if (!string.IsNullOrWhiteSpace(config.Key))
                    {
                        planeConfigByKey[config.Key] = config;
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: Offset JSON file not found.");
            }
        }

        public static PlaneConfig GetPlaneConfig(string aircraftType)
        {
            if (!planeConfigByKey.TryGetValue(aircraftType, out PlaneConfig planeConfig))
            {
                // fallback to DEFAULT
                if (planeConfigByKey.TryGetValue("DEFAULT", out var defaultConfig))
                {
                    planeConfig = defaultConfig;
                }
                else
                {
                    planeConfig = new PlaneConfig { Key = "DEFAULT", GearOffset = -0.45f }; // hard fallback
                }
            }
            return planeConfig;
        }

        public static float GetGearOffset(string aircraftType)
        {
            return GetPlaneConfig(aircraftType).GearOffset;
        }
    }

    public class PlaneConfig
    {
        public string Key { get; set; } = "";
        public float GearOffset { get; set; } = 0.0f;
        // add other properties as needed, e.g.
        public string DisplayName { get; set; } = "";
        public bool IsTaildragger { get; set; } = true;
        public string Class { get; set; } = "";
        // Colision Point Index
        public uint CollisionNoseIndex { get; set; } = 10;
        public uint CollisionPropIndex { get; set; } = 11;
        public uint CollisionWheelLeftIndex { get; set; } = 1;
        public uint CollisionWheelNoseTailIndex { get; set; } = 0;
        public uint CollisionWheelRightIndex { get; set; } = 2;
        public uint CollisionWheelWingtip1Index { get; set; } = 3;
        public uint CollisionWheelWingtip2Index { get; set; } = 4;
    }
}
