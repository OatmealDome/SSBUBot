using System;
using Newtonsoft.Json;

namespace SmashBcatDetector.Core.Config.NintendoCdn
{
    public class CachedToken
    {
        public string Token
        {
            get;
            set;
        }
        
        public DateTime ExpiryTime
        {
            get;
            set;
        }

    }
}