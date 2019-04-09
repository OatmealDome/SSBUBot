using BcatBotFramework.Core.Config;

namespace SmashBcatDetector.Core.Config
{
    public class WebConfig : ISubConfiguration
    {
        public string ContainerIndexPath
        {
            get;
            set;
        }

        public string ContainerListPath
        {
            get;
            set;
        }

        public void SetDefaults()
        {
            ContainerIndexPath = "/home/oatmealdome/www/index.json";
            ContainerListPath = "/home/oatmealdome/www/list_{0}.json";
        }

    }
}
