using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Quartz;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference.Handlers.Archival;
using SmashBcatDetector.Json.Config;
using SmashBcatDetector.Json.Config.Discord;
using SmashBcatDetector.Social;
using SmashBcatDetector.Util;
using SmashUltimate.Bcat;

namespace SmashBcatDetector.Scheduler.Job
{
    public class BootHousekeepingJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Scheduling archivals");

            // Archive all Container pages in production mode
            if (Configuration.LoadedConfiguration.IsProduction)
            {
                // Declare the minutes offset to stagger the archival jobs
                int minutesOffset = 1;

                // Get the current DateTime in UTC
                DateTime nowTime = DateTime.UtcNow;

                async Task ScheduleArchivalIfEligible(SmashUltimate.Bcat.Container container)
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

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Processing joined/left guilds");

            // Get a list of guilds
            IReadOnlyCollection<SocketGuild> socketGuilds = DiscordBot.GetGuilds();

            // Get all guild IDs that we have settings for
            List<ulong> configurationGuilds = new List<ulong>();
            foreach (GuildSettings guildSettings in Configuration.LoadedConfiguration.DiscordConfig.GuildSettings)
            {
                configurationGuilds.Add(guildSettings.GuildId);
            }

            // Get all IDs for guilds that we are in
            List<ulong> currentGuilds = new List<ulong>();
            foreach (SocketGuild socketGuild in DiscordBot.GetGuilds())
            {
                currentGuilds.Add(socketGuild.Id);
            }

            // Get all the guilds we have joined
            IEnumerable<ulong> joinedGuilds = currentGuilds.Except(configurationGuilds);
            foreach (ulong id in joinedGuilds)
            {
                // TODO: find a better solution instead of spamming the Welcome message
                //await DiscordUtil.ProcessJoinedGuild(socketGuilds.Where(guild => guild.Id == id).FirstOrDefault());
            }

            // Get all the guilds we have been removed from
            IEnumerable<ulong> removedGuilds = configurationGuilds.Except(currentGuilds);
            foreach (ulong id in removedGuilds)
            {
                await DiscordUtil.ProcessLeftGuild(id, null);
            }

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Processing deregistrations because of no write permissions");

            await DiscordUtil.FindBadGuilds();

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Saving configuration");

            // Save the configuration
            Configuration.LoadedConfiguration.Write();

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[BootHousekeepingJob]** Scheduling immediate BCAT check");

            // Schedule a BCAT check now
            await QuartzScheduler.ScheduleJob<BcatCheckerJob>("Immediate");
        }
        
    }
}