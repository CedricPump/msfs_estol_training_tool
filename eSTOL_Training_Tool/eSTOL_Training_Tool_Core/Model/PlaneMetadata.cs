using Newtonsoft.Json;

namespace eSTOL_Training_Tool
{
    public class PlaneMetadata
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;
    }
}
