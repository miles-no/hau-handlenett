namespace HandlenettAPI.Helpers
{
    public static class AzureSQLSetup
    {
        private static bool IsRunningInAzure()
        {
            return Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") != null;
        }
    }
}
