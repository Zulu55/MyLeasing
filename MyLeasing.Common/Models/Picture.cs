using Newtonsoft.Json;

namespace MyLeasing.Common.Models
{
    public class Picture
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
