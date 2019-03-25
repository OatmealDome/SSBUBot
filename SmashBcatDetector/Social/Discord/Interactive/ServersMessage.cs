using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SmashBcatDetector.Social.Discord.Interactive
{
    public class ServersMessage : PagedInteractiveMessage
    {
        private List<IEnumerable<SocketGuild>> guildLists;
        private int totalGuilds;

        protected override int LastPage
        {
            get
            {
                return guildLists.Count - 1;
            }
        }

        public ServersMessage(IUser user) : base(user)
        {
            // Get the guilds
            IReadOnlyCollection<SocketGuild> guilds = DiscordBot.GetGuilds();
            totalGuilds = guilds.Count;

            // Create the list of lists
            guildLists = new List<IEnumerable<SocketGuild>>();

            // Loop over every 10 entries
            for (int i = 0; i < guilds.Count; i += 10)
            {
                // Take 10 entries from the list
                guildLists.Add(guilds.Skip(i).Take(10));
            }
        }

        public override MessageProperties CreateMessageProperties()
        {
            // Construct the description
            string description = "```";

            // Loop over every guild in the current list
            foreach (SocketGuild guild in guildLists[this.CurrentPage])
            {
                description += $"{guild.Name} ({guild.Id})\n";
            }

            description += "```";

            // Construct the footer
            EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder()
                .WithText($"Page {this.CurrentPage + 1} / {this.LastPage}, found {totalGuilds} total");

            // Construct the embed
            Embed embed = new EmbedBuilder()
                .WithTitle("Server List")
                .WithAuthor(this.User)
                .WithDescription(description)
                .WithFooter(footerBuilder)
                .WithColor(Color.Blue)
                .Build();
            
            // Return the properties
            return new MessageProperties()
            {
                Embed = embed
            };
        }

    }
}