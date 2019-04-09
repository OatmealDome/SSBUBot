using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Social.Discord;
using SmashBcatDetector.Util;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class InviteCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Invite"), Summary("Gives instructions on how to invite the bot to your server")]
        public async Task Execute()
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context.Guild, null);
            
            // Create the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(Localizer.Localize("invite.title", language))
                .WithDescription(Localizer.Localize("invite.description", language))
                .WithColor(Color.Green)
                .Build();
            
            await Context.Channel.SendMessageAsync(embed: embed);
        }
    }
}