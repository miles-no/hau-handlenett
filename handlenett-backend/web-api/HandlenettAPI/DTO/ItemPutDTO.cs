using Newtonsoft.Json;

namespace HandlenettAPI.DTO
{
    public class ItemPutDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}
