﻿namespace HandlenettAPI.Models
{
    using Newtonsoft.Json;

    public class Item : Base
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}
