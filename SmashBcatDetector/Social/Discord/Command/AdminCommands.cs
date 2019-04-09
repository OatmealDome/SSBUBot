using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference.Handlers.Web;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using SmashBcatDetector.Scheduler;
using SmashBcatDetector.Scheduler.Job;
using SmashBcatDetector.Util;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Social.Discord.Precondition;
using BcatBotFramework.Scheduler;
using BcatBotFramework.Social.Discord;
using SmashBcatDetector.Difference.Handlers.Discord;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [RequireBotAdministratorPrecondition]
        [Command("announce"), Summary("Announces a message to all channels")]
        public async Task Announce(string announcement)
        {
            await DiscordBot.SendNotificationAsync($"**[SSBUBot Administrator Announcement]**\n\n{announcement}");

            await Context.Channel.SendMessageAsync("**[Admin]** OK, announced");
        }

        [RequireBotAdministratorPrecondition]
        [Command("announcetochannel"), Summary("Announces a message to the specified channel")]
        public async Task AnnounceToChannel(ulong guildId, ulong channelId, string announcement)
        {
            await DiscordBot.GetChannel(guildId, channelId).SendMessageAsync($"**[SSBUBot Administrator Message]**\n\n{announcement}");

            await Context.Channel.SendMessageAsync("**[Admin]** OK, announced");
        }

        [RequireBotAdministratorPrecondition]
        [Command("notify"), Summary("Sends a notification about the specified container")]
        public async Task AnnounceToChannel(string type, string id)
        {
            switch (type)
            {
                case "event":
                    await EventDiscordHandler.HandleAdded(ContainerCache.GetEventWithId(id));
                    break;
                case "linenews":
                    await LineNewsDiscordHandler.HandleAdded(ContainerCache.GetLineNewsWithId(id));
                    break;
                case "popupnews":
                    await PopUpNewsDiscordHandler.HandleAdded(ContainerCache.GetPopUpNewsWithId(id));
                    break;
                case "present":
                    await PresentDiscordHandler.HandleAdded(ContainerCache.GetPresentWithId(id));
                    break;
                default:
                    throw new Exception("Invalid type (must be event, linenews, popupnews, or present)");
            }

            await Context.Channel.SendMessageAsync("**[Admin]** OK, notified");
        }

        [RequireBotAdministratorPrecondition]
        [Command("saveconfig"), Summary("Saves the configuration")]
        public async Task SaveConfig()
        {
            Configuration.LoadedConfiguration.Write();

            await Context.Channel.SendMessageAsync("**[Admin]** OK, configuration saved");
        }

        [RequireBotAdministratorPrecondition]
        [Command("checknow"), Summary("Checks BCAT")]
        public async Task ForceBcatCheck()
        {
            await Context.Channel.SendMessageAsync("**[Admin]** OK, scheduling immediate BCAT check");

            await QuartzScheduler.ScheduleJob<BcatCheckerJob>("Immediate");
        }

        [RequireBotAdministratorPrecondition]
        [Command("reloadcontainercache"), Summary("Reloads ContainerCache")]
        public async Task ReloadContainerCache()
        {
            await Context.Channel.SendMessageAsync("**[Admin]** OK, reloading ContainerCache");

            ContainerCache.Dispose();
            ContainerCache.Initialize(Program.LOCAL_CONTAINER_CACHE_DIRECTORY);
        }

        [RequireBotAdministratorPrecondition]
        [Command("uploadcachetos3"), Summary("Uploads all ContainerCache files to S3")]
        public async Task UploadCacheToS3()
        {
            await Context.Channel.SendMessageAsync("**[Admin]** OK, building list");

            List<Container> allContainers = new List<Container>();

            // Add all Containers to the List
            allContainers.AddRange(ContainerCache.GetEvents());
            allContainers.AddRange(ContainerCache.GetLineNews());
            allContainers.AddRange(ContainerCache.GetPopUpNews());
            allContainers.AddRange(ContainerCache.GetPresents());

            // Loop over every Container
            foreach (Container container in allContainers)
            {
                await Context.Channel.SendMessageAsync("**[Admin]** Uploading " + container.GetType().Name + " with ID " + container.Id);

                ContainerWebHandler.HandleContainer(container);
            }

            await Context.Channel.SendMessageAsync("**[Admin]** Done");
        }

        [RequireBotAdministratorPrecondition]
        [Command("shutdown"), Summary("Shuts down the bot")]
        public async Task Shutdown(bool shouldRestart = true)
        {
            if (!shouldRestart && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Write out the automatic restart disabling file
                File.WriteAllText(Program.LOCAL_AUTOMATIC_RESTART_DISABLE_FLAG, "no restart");
            }

            await Context.Channel.SendMessageAsync("**[Admin]** OK, scheduling immediate shutdown");

            await QuartzScheduler.ScheduleJob<SsbuBotShutdownJob>("Immediate");
        }
        
    }
}