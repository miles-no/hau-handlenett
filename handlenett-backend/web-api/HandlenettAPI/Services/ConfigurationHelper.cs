namespace HandlenettAPI.Services
{
    public static class ConfigurationHelper
    {
        public static readonly IConfiguration config; //for å ha tilgang til config i statc classes
        public static void Initialize(IConfiguration Configuration)
        {
            //config = Configuration;
        }
    }
}
