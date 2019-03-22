using System.Threading.Tasks;
using Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Internationalization;
using SmashBcatDetector.Util;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class InviteCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Invite"), Summary("Gives instructions on how to invite the bot to your server")]
        public async Task Execute()
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, null);

            // Localize the title
            string titleKey = "Invite";
            string localizedTitle = Localizer.Localize(titleKey, language);

            // Localize the description
            string descriptionKey = "Please go to [https://smash.oatmealdome.me/setup](https://smash.oatmealdome.me/setup) for instructions on how to invite SSBUBot to your server.";
            string localizedDescription = Localizer.Localize(descriptionKey, language);

            // Create the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(localizedTitle)
                .WithDescription(localizedDescription)
                .WithColor(Color.Green)
                .Build();
            
            await Context.Channel.SendMessageAsync(embed: embed);
        }
    }
}