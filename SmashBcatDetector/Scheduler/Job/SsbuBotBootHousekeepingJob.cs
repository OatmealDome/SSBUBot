using System;
using System.IO;
using System.Threading.Tasks;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Scheduler;
using BcatBotFramework.Scheduler.Job;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference.Handlers.Archival;

namespace SmashBcatDetector.Scheduler.Job
{
    public class SsbuBotBootHousekeepingJob : BootHousekeepingJob
    {
        protected override async Task RunAppSpecificBootTasks()
        {
            // Create the cache directories if needed
            Directory.CreateDirectory(Program.LOCAL_CONTAINER_CACHE_DIRECTORY);
            Directory.CreateDirectory(Program.LOCAL_COMMON_CACHE_DIRECTORY);
            
            // Initialize the ContainerCache
            ContainerCache.Initialize(Program.LOCAL_CONTAINER_CACHE_DIRECTORY);

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Scheduling archivals");

            // Archive all Container pages in production mode
            if (Configuration.LoadedConfiguration.IsProduction)
            {
                // Declare the minutes offset to stagger the archival jobs
                int minutesOffset = 1;

                // Get the current DateTime in UTC
                DateTime nowTime = DateTime.UtcNow;

                async Task ScheduleArchivalIfEligible(Nintendo.SmashUltimate.Bcat.Container container)
                {
                    // Check if the Container's page is active
                    if (container.StartDateTime < nowTime && container.EndDateTime > nowTime)
                    {
                        // Schedule the Job
                        if (await ContainerArchivalHandler.ScheduleArchival(container, nowTime.AddMinutes(minutesOffset)))
                        {
                            // Increment the minutes
                            minutesOffset++;
                        }
                    }
                }

                // Schedule for all PopUpNews and Events
                foreach (PopUpNews popUpNews in ContainerCache.GetPopUpNews())
                {
                    await ScheduleArchivalIfEligible(popUpNews);
                }

                foreach (Event smashEvent in ContainerCache.GetEvents())
                {
                    await ScheduleArchivalIfEligible(smashEvent);
                }
            }
        }

        protected override async Task SchedulePostBootJobs()
        {
            // Check if this is the production bot
            if (Configuration.LoadedConfiguration.IsProduction)
            {
                // Schedule regular BCAT checks
                await QuartzScheduler.ScheduleJob<BcatCheckerJob>("Regular", Configuration.LoadedConfiguration.JobSchedules["Bcat"]);

                await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Scheduling immediate BCAT check");

                // Schedule a BCAT check now
                await QuartzScheduler.ScheduleJob<BcatCheckerJob>("Immediate");
            }

            // Schedule the recurring housekeeping job
            await QuartzScheduler.ScheduleJob<SsbuBotRecurringHousekeepingJob>("Regular", Configuration.LoadedConfiguration.JobSchedules["Housekeeping"]);
        }

    }
}