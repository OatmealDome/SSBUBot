using System.Collections.Concurrent;
using System.Collections.Generic;
using Nintendo.Bcat;
using SmashBcatDetector.Json.Config.Discord;

namespace SmashBcatDetector.Json.Config
{
    public class DiscordConfig
    {
        public string Token
        {
            get;
            set;
        }

        public ulong ClientId
        {
            get;
            set;
        }

        public uint Permissions
        {
            get;
            set;
        }

        public List<ulong> AdministratorIds
        {
            get;
            set;
        }

        public GuildSettings LoggingTargetChannel
        {
            get;
            set;
        }

        public List<GuildSettings> GuildSettings
        {
            get;
            set;
        }

        public int AlternatorRate
        {
            get;
            set;
        }

        public int MessageCacheSize
        {
            get;
            set;
        }

        public ConcurrentDictionary<string, ulong> CommandStatistics
        {
            get;
            set;
        }
        
    }
}