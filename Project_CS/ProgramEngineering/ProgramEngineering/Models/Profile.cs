using Newtonsoft.Json;

namespace ProgramEngineering.Models
{
    public class Profile
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("boughtPicturesCount")]
        public int BoughtPicturesCount { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
