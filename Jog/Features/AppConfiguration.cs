using System.Configuration;
using Nancy;

namespace Jog.Features
{
    public class AppConfiguration
    {
        private static readonly DynamicDictionary Config = new DynamicDictionary();

        static AppConfiguration()
        {
            //initializing from configuraiton settings
            foreach (var appKey in ConfigurationManager.AppSettings.AllKeys)
            {
                Config[appKey] = ConfigurationManager.AppSettings[appKey];
            }
        }

        public static dynamic Current
        {
            get { return Config; }
        }

        private AppConfiguration()
        {
        }
    }
}
