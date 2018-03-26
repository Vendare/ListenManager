using ListenManager.Database.Handlers;

namespace ListenManager.Config
{
    public class ConfigHandler
    {
        private static ConfigHandler _instance;

        public static ConfigHandler Instance => _instance ?? (_instance = new ConfigHandler());

        private ConfigHandler()
        {
            var handler = VerzeichnisHandler.Instance;
        }
    }
}
