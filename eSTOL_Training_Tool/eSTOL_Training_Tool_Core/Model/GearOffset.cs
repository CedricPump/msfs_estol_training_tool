using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace eSTOL_Training_Tool_Core.Model
{
    public static class GearOffset
    {
        private static Dictionary<string, float> offsetDict = new();
        private static float defaultOffset = -0.45f;

        public static void LoadOffsetDict(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                offsetDict = JsonSerializer.Deserialize<Dictionary<string, float>>(json) ?? new();
            }
            else
            {
                Console.WriteLine("Error: Offset JSON file not found.");
            }
        }

        public static float getGearOffset(string aircraftType) 
        {
            return offsetDict.GetValueOrDefault(aircraftType, defaultOffset);
        }



    }
}
