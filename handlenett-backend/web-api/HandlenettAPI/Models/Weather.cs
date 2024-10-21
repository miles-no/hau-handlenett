namespace HandlenettAPI.Models
{
    public class Weather
    {
        public string Temperature { get; set; }
        public Uri ImageUri { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
