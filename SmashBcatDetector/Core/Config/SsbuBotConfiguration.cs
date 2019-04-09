using BcatBotFramework.Core.Config;
using Newtonsoft.Json;

namespace SmashBcatDetector.Core.Config
{
    public class SsbuBotConfiguration : Configuration
    {
        [JsonProperty("Web")]
        public WebConfig WebConfig
        {
            get;
            set;
        }

        protected override void SetAppSpecificDefaults()
        {
            WebConfig = new WebConfig();
            WebConfig.SetDefaults();
        }

    }
}