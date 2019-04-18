using BcatBotFramework.Core.Config;
using SmashBcatDetector.Core.Config.Web;

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

        public RemoteServer RemoteServer
        {
            get;
            set;
        }

        public void SetDefaults()
        {
            ContainerIndexPath = "/home/oatmealdome/www/index.json";
            ContainerListPath = "/home/oatmealdome/www/list_{0}.json";
            RemoteServer = null;
        }

    }
}
