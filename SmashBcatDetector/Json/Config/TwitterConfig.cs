using System.Collections.Generic;
using SmashBcatDetector.Json.Config.Twitter;

namespace SmashBcatDetector.Json.Config
{
    public class TwitterConfig
    {
        public Dictionary<string, CachedTwitterCredentials> TwitterCredentials
        {
            get;
            set;
        }
        
        public string CharacterCounterBinary
        {
            get;
            set;
        }

        public bool IsActivated
        {
            get;
            set;
        }
        
    }
}