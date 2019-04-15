using System;
using System.Linq;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using BcatBotFramework.Social.Discord;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class RegistrationCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Register"), Summary("Registers your server for notifications")]
        public async Task Register(string languageCode)
        {
            await Register(Context.Channel as IGuildChannel, languageCode);
        }

        [Command("Register"), Summary("Registers your server for notifications")]
        public async Task Register(IGuildChannel channel, string languageCode)
        {
            if (Context.Guild == null)
            {
                throw new LocalizedException("registration.in_dm");
            }

            // Check that that the user has the manage guild permission
            if (!((SocketGuildUser)Context.User).GuildPermissions.Has(GuildPermission.ManageGuild))
            {
                throw new LocalizedException("registration.user_no_manage_permission");
            }

            // Check that we can write to this channel first
            if (!Context.Guild.CurrentUser.GetPermissions(channel).Has(ChannelPermission.SendMessages))
            {
                throw new LocalizedException("registration.bot_no_write_permission");
            }

            // Check the language code
            Language language;
            try
            {
                language = LanguageExtensions.FromCode(languageCode);
            }
            catch (Exception)
            {
                throw new LocalizedException("registration.bad_code");
            }

            // Get any existing GuildSettings for this server
            GuildSettings guildSettings = Configuration.LoadedConfiguration.DiscordConfig.GuildSettings.Where(x => x.GuildId == Context.Guild.Id).FirstOrDefault();

            // Check if the GuildSettings doesn't exist
            if (guildSettings == null)
            {
                // Create a GuildSettings instance
                guildSettings = new GuildSettings();

                // Add this to the Configuration
                Configuration.LoadedConfiguration.DiscordConfig.GuildSettings.Add(guildSettings);
            }

            // Set the GuildSettings fields
            guildSettings.GuildId = Context.Guild.Id;
            guildSettings.TargetChannelId = channel.Id;
            guildSettings.DefaultLanguage = language;

            // Get the localized embed fields
            string embedTitle = Localizer.Localize("registration.title", guildSettings.DefaultLanguage);
            string embedDescription = Localizer.Localize("registration.description", guildSettings.DefaultLanguage);

            // Build the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(embedTitle)
                .WithDescription(embedDescription)
                .WithColor(Color.Green)
                .Build();

            // Send the Embed
            await Context.Channel.SendMessageAsync(embed: embed);

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[RegistrationCommand]** Registered \"{Context.Guild.Name}\" ({Context.Guild.Id}) to #{channel.Name} ({channel.Id}) using language {language.ToString()}");
        }
        
    }
}