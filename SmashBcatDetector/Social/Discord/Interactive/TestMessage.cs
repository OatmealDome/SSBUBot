using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SmashBcatDetector.Social.Discord.Interactive
{
    public class TestMessage : InteractiveMessage
    {
        private List<IEnumerable<int>> guildLists;
        private int currentPage = 0;

        // Emotes
        private IEmote EMOTE_TO_BEGINNING = new Emoji("\u23EA"); // ⏪
        private IEmote EMOTE_BACK = new Emoji("\u25C0"); // ◀️
        private IEmote EMOTE_FORWARD = new Emoji("\u25B6"); // ▶️
        private IEmote EMOTE_TO_END = new Emoji("\u23E9"); // ⏩

        public TestMessage()
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
            foreach (int guild in guildLists[currentPage])
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

        public override bool HandleReaction(IEmote emote)
        {
            if (emote.Name == EMOTE_TO_BEGINNING.Name)
            {
                currentPage = 0;
                return true;
            }
            else if (emote.Name == EMOTE_BACK.Name && currentPage > 0)
            {
                currentPage--;
                return true;
            }
            else if (emote.Name == EMOTE_FORWARD.Name && currentPage < guildLists.Count)
            {
                currentPage++;
                return true;
            }
            else if (emote.Name == EMOTE_TO_END.Name)
            {
                currentPage = guildLists.Count - 1;
                return true;
            }

            return false;
        }

        public override async Task AddReactions(SocketReaction reaction)
        {
            if (currentPage == 0)
            {
                await targetMessage.RemoveAllReactionsAsync();

                await targetMessage.AddReactionAsync(EMOTE_FORWARD);
                await targetMessage.AddReactionAsync(EMOTE_TO_END);
            }
            else if ((currentPage == 1 && !HasReaction(EMOTE_BACK)) || (currentPage == guildLists.Count - 2 && !HasReaction(EMOTE_FORWARD))) // first page or second to last page
            {
                await targetMessage.RemoveAllReactionsAsync();

                await targetMessage.AddReactionAsync(EMOTE_TO_BEGINNING);
                await targetMessage.AddReactionAsync(EMOTE_BACK);
                await targetMessage.AddReactionAsync(EMOTE_FORWARD);
                await targetMessage.AddReactionAsync(EMOTE_TO_END);
            }
            else if (currentPage == guildLists.Count - 1)
            {
                await targetMessage.RemoveAllReactionsAsync();

                await targetMessage.AddReactionAsync(EMOTE_TO_BEGINNING);
                await targetMessage.AddReactionAsync(EMOTE_BACK);
            }
            else
            {
                // Clear the user's reaction on this message
                await targetMessage.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
        }

    }
}