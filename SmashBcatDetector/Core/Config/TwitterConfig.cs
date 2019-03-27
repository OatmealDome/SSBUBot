using System.Collections.Generic;
using SmashBcatDetector.Core.Config.Twitter;

namespace SmashBcatDetector.Core.Config
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