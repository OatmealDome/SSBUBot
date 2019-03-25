using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SmashBcatDetector.Json.Config;
using SmashBcatDetector.Json.Config.Discord;
using SmashBcatDetector.Social.Discord.Interactive;
using SmashBcatDetector.Util;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class StatisticsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("lookup"), Summary("Looks up server information")]
        public async Task ServerLookup(ulong id)
        {
            DiscordUtil.CheckAdministratorPermission(Context.User.Id);
            
            // Get the GuildSettings
            GuildSettings guildSettings = Configuration.LoadedConfiguration.DiscordConfig.GuildSettings.Where(settings => settings.GuildId == id).FirstOrDefault();

            // Get the SocketGuild and SocketGuildChannel
            SocketGuild guild = DiscordBot.GetGuild(id);

            // Build an embed
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("Lookup")
                .AddField("Guild", $"{guild.Name} ({guild.Id}, {(guildSettings != null ? "registered" : "unregistered")})");

            // Check if this guild is registered
            if (guildSettings != null)
            {
                // Get the registered channel
                SocketGuildChannel channel = guild.GetChannel(guildSettings.TargetChannelId);

                // Add additional lookup details
                embedBuilder
                    .AddField("Channel", $"#{channel.Name} ({channel.Id})")
                    .AddField("Language", guildSettings.DefaultLanguage.ToString());                
            }
            
            // Complete the embed
            embedBuilder
                .AddField("Owner", $"{guild.Owner.Username}#{guild.Owner.Discriminator} ({guild.Owner.Id})")
                .AddField("User Count", guild.MemberCount)
                .WithColor(Color.Blue);

            await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("serverlist"), Summary("Lists all servers")]
        public async Task ServerList()
        {
            DiscordUtil.CheckAdministratorPermission(Context.User.Id);

            await DiscordBot.SendInteractiveMessageAsync(Context.Channel, new ServersMessage());
        }

        [Command("commandstats"), Summary("Lists statistics for commands")]
        public async Task CommandStats(string command = null)
        {
            DiscordUtil.CheckAdministratorPermission(Context.User.Id);

            // Check if a command was specified
            if (command != null)
            {
                // Create an embed
                Embed embed = new EmbedBuilder()
                    .WithTitle("Command Statistics")
                    .WithDescription($"``ssbu.{command}`` has been called {Configuration.LoadedConfiguration.DiscordConfig.CommandStatistics[command]} times.")
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .Build();

                // Send it
                await Context.Channel.SendMessageAsync(embed: embed);
            }
            else
            {
                // Get the top 10 commands
                IEnumerable<KeyValuePair<string, ulong>> topTen = Configuration.LoadedConfiguration.DiscordConfig.CommandStatistics
                    .OrderBy(x => x.Value).TakeLast(10).Reverse();

                // Create the description
                string description = "Here are the top 10 commands.\n\n";

                // Add each command
                foreach (KeyValuePair<string, ulong> pair in topTen)
                {
                    description += $"``ssbu.{pair.Key}``: {pair.Value}\n";
                }

                // Create an embed
                Embed embed = new EmbedBuilder()
                    .WithTitle("Command Statistics")
                    .WithDescription(description)
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .WithColor(Color.Blue)
                    .Build();

                // Send it
                await Context.Channel.SendMessageAsync(embed: embed);
            }
        }

    }
}