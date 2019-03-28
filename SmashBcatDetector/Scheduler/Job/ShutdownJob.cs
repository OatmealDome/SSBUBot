using System;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Nintendo.DAuth;
using Quartz;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference;
using BcatBotFramework.Core.Config;
using S3;
using SmashBcatDetector.Social;
using BcatBotFramework.Difference;

namespace SmashBcatDetector.Scheduler.Job
{
    public class ShutdownJob : IJob
    {
        private static int ShutdownWaitTime = 5; // in seconds

        public async Task Execute(IJobExecutionContext context)
        {
            // Shutdown the Scheduler
            await QuartzScheduler.Dispose();

            // Shutdown the DiscordBot
            await DiscordBot.Dispose();

            // Shutdown Twitter
            TwitterManager.Dispose();

            // Shutdown S3
            S3Api.Dispose();

            // Shutdown BCAT
            BcatApi.Dispose();

            // Shutdown DAuth
            DAuthApi.Dispose();

            // Shutdown the HandlerMapper
            HandlerMapper.Dispose();

            // Shutdown the ContainerCache
            ContainerCache.Dispose();

            // Save the configuration
            Configuration.LoadedConfiguration.Write();

            // Wait a little while just in case
            await Task.Delay(1000 * ShutdownWaitTime);

            Environment.Exit(0);
        }

    }
}