﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eSTOL_Training_Tool_Core.Core
{
    internal class VersionHelper
    {
        private const string currentVersion = "v1.2.0";
        private const string githubApiUrl = "https://api.github.com/repos/CedricPump/msfs_estol_training_tool/releases/latest";

        public static async Task<string> CheckForUpdateAsync()
        {
            using HttpClient client = new HttpClient();

            // GitHub API requires a user-agent header
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApp", "1.0"));

            try
            {
                HttpResponseMessage response = await client.GetAsync(githubApiUrl);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);
                string latestVersion = doc.RootElement.GetProperty("tag_name").GetString();

                if (IsNewerVersion(latestVersion, currentVersion))
                {
                    return latestVersion;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for update: " + ex.Message);
                return null;
            }
        }

        private static bool IsNewerVersion(string latest, string current)
        {
            Version latestV = ParseVersion(latest);
            Version currentV = ParseVersion(current);

            return latestV > currentV;
        }

        private static Version ParseVersion(string versionString)
        {
            versionString = versionString.TrimStart('v', 'V');
            return Version.TryParse(versionString, out var version) ? version : new Version(0, 0, 0);
        }
    }
}
