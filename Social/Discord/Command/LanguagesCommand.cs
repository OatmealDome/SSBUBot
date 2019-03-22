using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class LanguagesCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Languages"), Summary("Shows a list of languages")]
        public async Task Execute()
        {
            Embed embed = new EmbedBuilder()
                .WithTitle("Languages")
                .WithDescription("Here are all the languages supported by Smash and their IDs:```\nEnglish (American) - en-US\nFrançais (Canadien) - fr-CA\nEspañol (América Latina) - es-419\nEnglish (British) - en-GB\nFrançais - fr\nNederlands - nl\nDeutsch - de\nItaliano - it\nPусский - ru\nEspañol - es\n日本語 - ja\n조선말 - ko\n簡體中文 - zh-CN\n繁體中文 - zh-TW```")
                .WithColor(Color.Green)
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }

    }
}