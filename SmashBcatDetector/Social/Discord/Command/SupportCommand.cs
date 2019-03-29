using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Social.Discord;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class SupportCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Support"), Summary("Tells you where to get support")]
        public async Task Support()
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, null);
            
            Embed embed = new EmbedBuilder()
                .WithTitle(Localizer.Localize("support.title", language))
                .WithDescription(Localizer.Localize("support.description", language))
                .WithColor(Color.Orange)
                .Build();
            
            await Context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}