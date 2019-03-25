using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SmashBcatDetector.Social.Discord.Interactive
{
    public abstract class PagedInteractiveMessage : InteractiveMessage
    {
        protected int CurrentPage
        {
            get;
            set;
        }

        protected abstract int LastPage
        {
            get;
        }
        
        // Emotes
        private IEmote EMOTE_TO_BEGINNING = new Emoji("\u23EA"); // ⏪
        private IEmote EMOTE_BACK = new Emoji("\u25C0"); // ◀️
        private IEmote EMOTE_FORWARD = new Emoji("\u25B6"); // ▶️
        private IEmote EMOTE_TO_END = new Emoji("\u23E9"); // ⏩

        public override bool HandleReaction(IEmote emote)
        {
            if (emote.Name == EMOTE_TO_BEGINNING.Name)
            {
                CurrentPage = 0;
                return true;
            }
            else if (emote.Name == EMOTE_BACK.Name)
            {
                CurrentPage--;

                // Don't go below zero
                if (CurrentPage <= 0)
                {
                    CurrentPage = 0;
                }
                
                return true;
            }
            else if (emote.Name == EMOTE_FORWARD.Name)
            {
                CurrentPage++;

                // Don't go above maximum
                if (CurrentPage > LastPage)
                {
                    CurrentPage = LastPage;
                }

                return true;
            }
            else if (emote.Name == EMOTE_TO_END.Name)
            {
                CurrentPage = LastPage;
                return true;
            }

            return false;
        }

        public override async Task AddReactions(SocketReaction reaction)
        {
            if (CurrentPage == 0 && LastPage > 1)
            {
                await targetMessage.RemoveAllReactionsAsync();

                await targetMessage.AddReactionAsync(EMOTE_FORWARD);
                await targetMessage.AddReactionAsync(EMOTE_TO_END);
            }
            else if ((CurrentPage == 1 && !HasReaction(EMOTE_BACK)) || (CurrentPage == LastPage - 1 && !HasReaction(EMOTE_FORWARD))) // first page or second to last page
            {
                await targetMessage.RemoveAllReactionsAsync();

                await targetMessage.AddReactionAsync(EMOTE_TO_BEGINNING);
                await targetMessage.AddReactionAsync(EMOTE_BACK);
                await targetMessage.AddReactionAsync(EMOTE_FORWARD);
                await targetMessage.AddReactionAsync(EMOTE_TO_END);
            }
            else if (CurrentPage == LastPage)
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