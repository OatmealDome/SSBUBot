using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SmashBcatDetector.Social.Discord.Interactive
{
    public class TestMessage : PagedInteractiveMessage
    {
        private List<IEnumerable<int>> guildLists;

        protected override int LastPage
        {
            get
            {
                return guildLists.Count - 1;
            }
        }

        public TestMessage(IUser user) : base(user)
        {
            // Get the guilds
            List<int> guilds = new List<int>();
            for (int i = 0; i < 54; i++)
            {
                guilds.Add(i);
            }

            // Create the list of lists
            guildLists = new List<IEnumerable<int>>();

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
            string description = "";

            // Loop over every guild in the current list
            foreach (int guild in guildLists[this.CurrentPage])
            {
                description += $"{guild}\n";
            }

            // Construct the embed
            Embed embed = new EmbedBuilder()
                .WithTitle("Server List")
                .WithDescription(description)
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