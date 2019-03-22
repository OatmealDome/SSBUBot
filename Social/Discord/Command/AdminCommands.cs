using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference.Handlers.Web;
using SmashBcatDetector.Json.Config;
using SmashBcatDetector.Json.Config.Discord;
using SmashBcatDetector.Scheduler;
using SmashBcatDetector.Scheduler.Job;
using SmashBcatDetector.Util;
using SmashUltimate.Bcat;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [Command("announce"), Summary("Announces a message to all channels")]
        public async Task Announce(string announcement)
        {
            CheckAdministratorPermission();

            await DiscordBot.SendNotificationAsync($"**[SSBUBot Administrator Announcement]**\n\n{announcement}");

            await Context.Channel.SendMessageAsync("**[Admin]** OK, announced");
        }

        [Command("announcetochannel"), Summary("Announces a message to the specified channel")]
        public async Task AnnounceToChannel(ulong guildId, ulong channelId, string announcement)
        {
            CheckAdministratorPermission();

            await DiscordBot.GetChannel(guildId, channelId).SendMessageAsync($"**[SSBUBot Administrator Message]**\n\n{announcement}");

            await Context.Channel.SendMessageAsync("**[Admin]** OK, announced");
        }

        [Command("saveconfig"), Summary("Saves the configuration")]
        public async Task SaveConfig()
        {
            CheckAdministratorPermission();

            Configuration.LoadedConfiguration.Write();

            await Context.Channel.SendMessageAsync("**[Admin]** OK, configuration saved");
        }

        [Command("checknow"), Summary("Checks BCAT")]
        public async Task ForceBcatCheck()
        {
            CheckAdministratorPermission();

            await Context.Channel.SendMessageAsync("**[Admin]** OK, scheduling immediate BCAT check");

            await QuartzScheduler.ScheduleJob<BcatCheckerJob>("Immediate");
        }

        [Command("reloadcontainercache"), Summary("Reloads ContainerCache")]
        public async Task ReloadContainerCache()
        {
            CheckAdministratorPermission();

            await Context.Channel.SendMessageAsync("**[Admin]** OK, reloading ContainerCache");

            ContainerCache.Dispose();
            ContainerCache.Initialize(Program.LOCAL_CONTAINER_CACHE_DIRECTORY);
        }

        [Command("uploadcachetos3"), Summary("Uploads all ContainerCache files to S3")]
        public async Task UploadCacheToS3()
        {
            CheckAdministratorPermission();

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

        [Command("shutdown"), Summary("Shuts down the bot")]
        public async Task Shutdown(bool shouldRestart = true)
        {
            CheckAdministratorPermission();

            if (!shouldRestart && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Write out the automatic restart disabling file
                File.WriteAllText(Program.LOCAL_AUTOMATIC_RESTART_DISABLE_FLAG, "no restart");
            }

            await Context.Channel.SendMessageAsync("**[Admin]** OK, scheduling immediate shutdown");

            await QuartzScheduler.ScheduleJob<ShutdownJob>("Immediate");
        }

        private void CheckAdministratorPermission()
        {
            if (!Configuration.LoadedConfiguration.DiscordConfig.AdministratorIds.Contains(Context.User.Id))
            {
                throw new LocalizedException("You are not allowed to do this");
            }
        }
        
    }
}