using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Quartz;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using SmashBcatDetector.Social;
using SmashBcatDetector.Social.Discord;
using SmashBcatDetector.Util;

namespace SmashBcatDetector.Scheduler.Job
{
    public class RecurringHousekeepingJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await DiscordBot.LoggingChannel.SendMessageAsync($"**[RecurringHousekeepingJob]** Processing deregistrations because of no write permissions");

            await DiscordUtil.FindBadGuilds();

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[RecurringHousekeepingJob]** Saving configuration");

            // Save the configuration
            Configuration.LoadedConfiguration.Write();
        }

    }
}