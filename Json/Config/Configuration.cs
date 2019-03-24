using System.IO;
using Newtonsoft.Json;

namespace SmashBcatDetector.Json.Config
{
    public class Configuration
    {
        public static Configuration LoadedConfiguration;
        private static string ConfigurationFilePath;

        [JsonProperty("NintendoCdn")]
        public NintendoCdnConfig CdnConfig
        {
            get;
            set;
        }

        [JsonProperty("Keyset")]
        public KeysetConfig KeysetConfig
        {
            get;
            set;
        }

        [JsonProperty("Discord")]
        public DiscordConfig DiscordConfig
        {
            get;
            set;
        }

        [JsonProperty("S3")]
        public S3Config S3Config
        {
            get;
            set;
        }

        [JsonProperty("Web")]
        public WebConfig WebConfig
        {
            get;
            set;
        }

        [JsonProperty("Scheduler")]
        public SchedulerConfig SchedulerConfig
        {
            get;
            set;
        }

        [JsonProperty("Twitter")]
        public TwitterConfig TwitterConfig
        {
            get;
            set;
        }

        public bool FirstRunCompleted
        {
            get;
            set;
        }

        public bool IsProduction
        {
            get;
            set;
        }

        public static void Load(string path) 
        {
            LoadedConfiguration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
            ConfigurationFilePath = path;
        }

        public void Write()
        {
            File.WriteAllText(ConfigurationFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

    }
}