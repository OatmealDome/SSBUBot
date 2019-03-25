using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace SmashBcatDetector.Social.Discord
{
    public abstract class InteractiveMessage
    {
        protected RestUserMessage targetMessage = null;
        protected IUser user;

        public ulong MessageId
        {
            get
            {
                return targetMessage != null ? targetMessage.Id : 0;
            }
        }

        protected InteractiveMessage(IUser user)
        {
            this.user = user;
        }

        public async Task SendInitialMessage(ISocketMessageChannel targetChannel)
        {
            // Check if we've already sent the initial message
            if (targetMessage != null)
            {
                throw new Exception("Cannot send initial message twice");
            }

            // Create the initial message
            MessageProperties properties = CreateMessageProperties();

            // Send the message
            targetMessage = await targetChannel.SendMessageAsync(text: properties.Content.GetValueOrDefault(), embed: properties.Embed.GetValueOrDefault());

            // Add the reactions
            await AddReactions(null);
        }

        public async Task ReactionAdded(SocketReaction reaction)
        {
            // Handle the reaction if needed and if it's from the 
            if (!HandleReaction(reaction.Emote))
            {
                return;
            }

            // Create the initial message
            MessageProperties newProperties = CreateMessageProperties();

            // Modify the message
            await targetMessage.ModifyAsync(properties =>
            {
                properties.Content = newProperties.Content;
                properties.Embed = newProperties.Embed;
            });

            // Add and clear any reactions if needed
            await AddReactions(reaction);
        }

        public async Task ClearReactions()
        {
            await targetMessage.RemoveAllReactionsAsync();
        }

        protected bool HasReaction(IEmote emote)
        {
            return targetMessage.Reactions.Count(x => x.Key.Name == emote.Name) > 0;
        }

        public abstract MessageProperties CreateMessageProperties();

        public abstract bool HandleReaction(IEmote emote);

        public abstract Task AddReactions(SocketReaction reaction);

    }
}