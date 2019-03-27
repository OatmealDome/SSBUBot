using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SmashBcatDetector.Core.Config;
using SmashBcatDetector.Core.Config.Discord;
using SmashBcatDetector.Social.Discord.Interactive;
using SmashBcatDetector.Social.Discord.Precondition;
using SmashBcatDetector.Util;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class StatisticsCommand : ModuleBase<SocketCommandContext>
    {
        [RequireBotAdministratorPrecondition]
        [Command("lookup"), Summary("Looks up server information")]
        public async Task ServerLookup(ulong id)
        {
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

        [RequireBotAdministratorPrecondition]
        [Command("serverlist"), Summary("Lists all servers")]
        public async Task ServerList()
        {
            await DiscordBot.SendInteractiveMessageAsync(Context.Channel, new ServersMessage(Context.User));
        }

        [RequireBotAdministratorPrecondition]
        [Command("commandstats"), Summary("Lists statistics for commands")]
        public async Task CommandStats(string command = null)
        {
            // Check if a command was specified
            if (command != null)
            {
                // Create an embed
                Embed embed = new EmbedBuilder()
                    .WithTitle("Command Statistics")
                    .WithDescription($"``{Configuration.LoadedConfiguration.DiscordConfig.CommandPrefix}{command}`` has been called {Configuration.LoadedConfiguration.DiscordConfig.CommandStatistics[command]} times.")
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
                    description += $"``{Configuration.LoadedConfiguration.DiscordConfig.CommandPrefix}{pair.Key}``: {pair.Value}\n";
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

        [RequireBotAdministratorPrecondition]
        [Command("stats"), Summary("Lists general statistics")]
        public async Task GeneralStats()
        {
            // Get the guilds
            IReadOnlyCollection<IGuild> guilds = DiscordBot.GetGuilds();

            // Count the total number of users
            int users = 0;
            foreach (SocketGuild guild in guilds)
            {
                users += guild.MemberCount;
            }

            // Calculate the number of registered servers
            int registeredServers = Configuration.LoadedConfiguration.DiscordConfig.GuildSettings.Count;
            int percent = (int)Math.Round((double)(100 * registeredServers) / guilds.Count);

            // Create an embed
            Embed embed = new EmbedBuilder()
                .WithTitle("General Statistics")
                .AddField("Servers", $"{guilds.Count}")
                .AddField("Registered Servers", $"{registeredServers} (approx. {percent}%)")
                .AddField("Total Users", users)
                .WithTimestamp(DateTimeOffset.UtcNow)
                .WithColor(Color.Blue)
                .Build();

            // Send the embed
            await Context.Channel.SendMessageAsync(embed: embed);
        }

    }
}