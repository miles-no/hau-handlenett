using Newtonsoft.Json;

namespace HandlenettAPI.DTO
{
    public class ItemPostDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
