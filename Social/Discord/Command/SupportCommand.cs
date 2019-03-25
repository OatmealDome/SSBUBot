using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Internationalization;
using SmashBcatDetector.Util;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class SupportCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Support"), Summary("Tells you where to get support")]
        public async Task Support()
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, null);

            // Localize the title
            string titleKey = "Support";
            string localizedTitle = Localizer.Localize(titleKey, language);

            // Localize the description
            string descriptionKey = "If you need a list of commands, type ``ssbu.help``. If you need help with using or setting up the bot, please join OatmealDome's server: https://discord.gg/rdx6Bt8";
            string localizedDescription = Localizer.Localize(descriptionKey, language);

            Embed embed = new EmbedBuilder()
                .WithTitle(localizedTitle)
                .WithDescription(localizedDescription)
                .WithColor(Color.Orange)
                .Build();
            
            await Context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}