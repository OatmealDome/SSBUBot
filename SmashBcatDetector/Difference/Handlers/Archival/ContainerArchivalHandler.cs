using System;
using System.Threading.Tasks;
using Quartz;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Scheduler;
using SmashBcatDetector.Scheduler.Job;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Difference;

namespace SmashBcatDetector.Difference.Handlers.Archival
{
    public class ContainerArchivalHandler
    {
        [SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Added, 100)]
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Added, 100)]
        public static async Task HandleContainer(Container container)
        {
            if (!Configuration.LoadedConfiguration.IsProduction)
            {
                return;
            }
            
            // Add 5 seconds to the start time
            DateTime dateTime = container.StartDateTime;
            dateTime.AddMinutes(5);

            await ScheduleArchival(container, dateTime);
        }

        public static async Task<bool> ScheduleArchival(Container container, DateTime dateTime)
        {
            // Check if this is an event PopUpNews
            PopUpNews popUpNews = container as PopUpNews;
            if (popUpNews != null && popUpNews.IsPopUpForEvent)
            {
                // Skip
                return false;
            }

            // Create the job name
            string jobName = container.GetType().Name + container.Id;

            // Get the Url
            string url = container.GetFormattedUrl();
            if (url.Length == 0)
            {
                // Skip
                return false;
            }

            // Create the JobDataMap
            JobDataMap dataMap = new JobDataMap();
            dataMap.Put(ContainerArchivalJob.ARCHIVAL_DATA_PREFIX, FileTypeExtensions.GetNamePrefixFromType(FileTypeExtensions.GetTypeFromContainer(container)));
            dataMap.Put(ContainerArchivalJob.ARCHIVAL_DATA_ID, container.Id);
            dataMap.Put(ContainerArchivalJob.ARCHIVAL_DATA_URL, url);

            // Schedule the job for this Container
            await QuartzScheduler.ScheduleJob<ContainerArchivalJob>(jobName, dateTime, dataMap);

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[ArchivalHandler]** Scheduled archival job for {container.GetType().Name} ({container.Id})");

            return true;
        }

    }
}