using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Summary("Shows a list of IDs")]
        public async Task Execute()
        {
            Embed embed = new EmbedBuilder()
                .WithTitle("Help")
                .WithDescription(@"Parameters in ``<>`` are required, while parameters in ``[]`` are optional. For a list of language codes, type ``ssbu.languages``.```
ssbu.popupnews <pop-up news ID> [language code]
ssbu.linenews <line news ID> [language code]
ssbu.event <event ID> [language code]
ssbu.present <present ID> [language code]
ssbu.list <""popupnews"" OR ""linenews"" OR ""event"" OR ""present"">
ssbu.languages
ssbu.invite
ssbu.support
ssbu.help```
Examples:```
ssbu.popupnews 2011
ssbu.present 1003 en-US
ssbu.list event```")
                .WithColor(Color.Green)
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }

    }
}