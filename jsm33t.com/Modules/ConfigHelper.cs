namespace jsm33t.com.Modules
{
    public static class ConfigHelper
    {
        private static readonly IConfigurationRoot _config;

        static ConfigHelper()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string NewConnectionString
        {
            get
            {
                return _config.GetConnectionString("jsmtConstr");
            }
        }
    }
}
