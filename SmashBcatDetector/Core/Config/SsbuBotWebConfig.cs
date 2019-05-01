using BcatBotFramework.Core.Config;

namespace SmashBcatDetector.Core.Config
{
    public class SsbuBotWebConfig : WebConfig
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

        public override void SetDefaults()
        {
            RemoteServer = null;
            ContainerIndexPath = "/home/oatmealdome/www/index.json";
            ContainerListPath = "/home/oatmealdome/www/list_{0}.json";
        }

    }
}