using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Bcat;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference;
using SmashBcatDetector.Difference.Handlers.Archival;
using SmashBcatDetector.Json.Config;
using SmashBcatDetector.Json.Config.Discord;
using SmashBcatDetector.S3;
using SmashBcatDetector.Scheduler;
using SmashBcatDetector.Scheduler.Job;
using SmashBcatDetector.Social;
using SmashUltimate.Bcat;

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
        public static string LOCAL_EXCEPTION_LOGS_DIRECTORY = Path.Combine(LOCAL_DIRECTORY, "ExceptionLogs");

        public static string FOLDER_DATE_TIME_FORMAT = "yyyy-MM-dd-HH-mm";

        public static string TOPIC_ID = "nx_data_01006a800016e000";
        public static string TITLE_ID = "01006A800016E000";
        public static string PASSPHRASE = "8ecc937a543d4dd1ce55ebb044a0f93c884262d8044f4b5c8763e6fc01dc5e85";

        static async Task Main(string[] args)
        {
            // Wait for the debugger to attach if requested
            if (args.Length > 0 && args[0].ToLower() == "--waitfordebugger")
            {
                Console.WriteLine("Waiting for debugger...");

                while (!Debugger.IsAttached)
                {
                    await Task.Delay(1000);
                }

                Console.WriteLine("Debugger attached!");
            }

            // Declare variable to hold the configuration
            Configuration configuration;

            // Check if the config file exists
            if (!File.Exists(LOCAL_CONFIGURATION))
            {
                // Create a new dummy Configuration
                configuration = new Configuration();

                // Set defaults
                configuration.BcatConfig = new BcatConfig();
                configuration.BcatConfig.SerialNumber = "XAW1000000000";
                configuration.BcatConfig.Environment = "lp1";
                configuration.DiscordConfig = new DiscordConfig();
                configuration.DiscordConfig.Token = "cafebabe";
                configuration.DiscordConfig.ClientId = 1234;
                configuration.DiscordConfig.Permissions = 0;
                configuration.DiscordConfig.AdministratorIds = new List<ulong>()
                {
                    112966101368901632, // OatmealDome
                    180994059542855681 // Simonx22
                };
                configuration.DiscordConfig.GuildSettings = new List<GuildSettings>();
                configuration.S3Config = new S3Config();
                configuration.S3Config.ServiceUrl = "https://s3.example.com";
                configuration.S3Config.BucketName = "bucket";
                configuration.S3Config.AccessKey = "cafebabe";
                configuration.S3Config.AccessKeySecret = "deadbeef";
                configuration.SchedulerConfig = new SchedulerConfig();
                configuration.TwitterConfig = new TwitterConfig();
                configuration.TwitterConfig.ConsumerKey = "cafebabe";
                configuration.TwitterConfig.ConsumerSecret = "deadbeef";
                configuration.TwitterConfig.Token = "cafebabe";
                configuration.TwitterConfig.TokenSecret = "deadbeef";
                configuration.FirstRunCompleted = false;

                // Write out the default config
                configuration.Write();

                Console.WriteLine("Wrote default configuration to " + LOCAL_CONFIGURATION);

                return;
            }

            // Create the common cache folder
            System.IO.Directory.CreateDirectory(Program.LOCAL_COMMON_CACHE_DIRECTORY);

            // Create the Exception logs directory
            System.IO.Directory.CreateDirectory(Program.LOCAL_EXCEPTION_LOGS_DIRECTORY);

            // Load the Configuration
            configuration = Configuration.Load(LOCAL_CONFIGURATION);

            // Initialize the ContainerCache
            ContainerCache.Initialize(LOCAL_CONTAINER_CACHE_DIRECTORY);

            // Initialize the HandlerMapper
            HandlerMapper.Initialize();

            // Initialize BCAT
            BcatApi.Initialize(configuration.BcatConfig.Environment, configuration.BcatConfig.SerialNumber);

            // Initialize S3
            S3Api.Initialize(configuration.S3Config.ServiceUrl, configuration.S3Config.BucketName,
                configuration.S3Config.AccessKey, configuration.S3Config.AccessKeySecret);

            // Initialize Twitter
            TwitterHandler.Initialize(configuration.TwitterConfig);

            // Initialize the DiscordBot
            await DiscordBot.Initialize(configuration.DiscordConfig);

            // Initialize the Scheduler
            await QuartzScheduler.Initialize(configuration.SchedulerConfig);

            // Wait for the bot to fully initialize
            while (!DiscordBot.IsReady)
            {
                await Task.Delay(1000);
            }

            // Print out to the logging channel that we're initialized
            await DiscordBot.LoggingChannel.SendMessageAsync("\\*\\*\\* **Initialized**");

            // Schedule the BootHousekeepingJob
            await QuartzScheduler.ScheduleJob<BootHousekeepingJob>("Immediate");
            
            await Task.Delay(-1);

        }

    }
}
