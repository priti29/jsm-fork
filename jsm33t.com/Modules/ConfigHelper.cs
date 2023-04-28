namespace jsm33t.com.Modules
{
    public static class ConfigHelper
    {
        private static readonly IConfigurationRoot _config;

        static ConfigHelper() => _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        #pragma warning disable CS8603 // Possible null reference return.
        public static string NewConnectionString => _config.GetConnectionString("jsmtConstr");
        #pragma warning restore CS8603 // Possible null reference return.
    }
}
