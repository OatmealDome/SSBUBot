using System;
using System.Collections.Generic;
using SmashBcatDetector.Json.Config.NintendoCdn;

namespace SmashBcatDetector.Json.Config
{
    public class NintendoCdnConfig
    {
        public string Certificate
        {
            get;
            set;
        }

        public string SerialNumber
        {
            get;
            set;
        }

        public string Environment
        {
            get;
            set;
        }

        public int MasterKey
        {
            get;
            set;
        }

        public string SystemDigest
        {
            get;
            set;
        }

        public Dictionary<string, CachedToken> CachedTokens
        {
            get;
            set;
        }

        public void SetToDefaults()
        {
            Certificate = "/home/oatmealdome/switch.pfx";
            SerialNumber = "XAW1000000000";
            Environment = "lp1";
            MasterKey = 0;
            SystemDigest = "CusHY";
            CachedTokens = new Dictionary<string, CachedToken>();
        }
        
    }
}