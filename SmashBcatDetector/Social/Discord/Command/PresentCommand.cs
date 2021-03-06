using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Core;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class PresentCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Present"), Summary("Shows the present with the specified ID")]
        public async Task Execute(string id = null, string languageString = null)
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context.Guild, languageString);

            // Check for no ID
            if (id == null)
            {
                await ListCommand.ListContainers(FileType.Present, language, Context);

                return;
            }

            // Get the Present with this ID
            Present present = ContainerCache.GetPresentWithId(id);

            // Check if this exists
            if (present == null)
            {
                throw new LocalizedException("present.not_found");
            }

            // Localize the description
            string localizedDescription = string.Format(Localizer.Localize("present.description", language), present.ContentText[language], $"https://smash.oatmealdome.me/present/{present.Id}/{language.GetCode()}/");

            // Construct the image URL
            string url = $"https://cdn.oatmealdome.me/smash/present/{present.Id}/image.jpg";

            // Construct the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(present.TitleText[language])
                .WithDescription(localizedDescription)
                .AddField(Localizer.Localize("present.start_time", language), Localizer.LocalizeDateTime(present.StartDateTime, language), true)
                .AddField(Localizer.Localize("present.end_time", language), Localizer.LocalizeDateTime(present.EndDateTime, language), true)
                .WithImageUrl(url)
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}