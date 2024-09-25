using Newtonsoft.Json;

namespace HandlenettAPI.Models
{
    public abstract class Base
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; } = "r";

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; } = "r";

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonProperty("updatedDate")]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
