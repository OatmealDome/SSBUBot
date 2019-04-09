using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Nintendo.DAuth;
using SmashBcatDetector.Core;
using BcatBotFramework.Difference;
using SmashBcatDetector.Difference.Handlers.Archival;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using BcatBotFramework.Core.Config.Scheduler;
using S3;
using BcatBotFramework.Scheduler;
using SmashBcatDetector.Scheduler.Job;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Core.Config.Twitter;
using SmashBcatDetector.Core.Config;
using SmashBcatDetector.Difference;
using BcatBotFramework.Social.Twitter;
using BcatBotFramework.Internationalization;

namespace SmashBcatDetector
{
    class Program
    {
        // Local Directory
        private static string LOCAL_DIRECTORY = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string LOCAL_CONFIGURATION = Path.Combine(LOCAL_DIRECTORY, "config.json");
        public static string LOCAL_LAST_TOPIC = Path.Combine(LOCAL_DIRECTORY, "last_topic");
        public static string LOCAL_AUTOMATIC_RESTART_DISABLE_FLAG = Path.Combine(LOCAL_DIRECTORY, "no_automatic_restart");
        public static string LOCAL_OLD_DATA_DIRECTORY = Path.Combine(LOCAL_DIRECTORY, "DownloadedData", "{0}");
        public static string LOCAL_CONTAINER_CACHE_DIRECTORY = Path.Combine(LOCAL_DIRECTORY, "ContainerCache");
        public static string LOCAL_COMMON_CACHE_DIRECTORY = Path.Combine(LOCAL_DIRECTORY, "CommonCache");
        
        public static string FOLDER_DATE_TIME_FORMAT = "yyyy-MM-dd-HH-mm";

        public static string TITLE_ID = "01006A800016E000";
        public static string PASSPHRASE = "8ecc937a543d4dd1ce55ebb044a0f93c884262d8044f4b5c8763e6fc01dc5e85";

    }
}
