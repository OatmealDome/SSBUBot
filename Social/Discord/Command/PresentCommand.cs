using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Core;
using SmashBcatDetector.Internationalization;
using SmashBcatDetector.Json.Config;
using SmashBcatDetector.Json.Config.Discord;
using SmashBcatDetector.Util;
using SmashUltimate.Bcat;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class PresentCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Present"), Summary("Shows the present with the specified ID")]
        public async Task Execute(string id = null, string languageString = null)
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, languageString);

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
                throw new LocalizedException("No present exists with this ID");
            }

            // Localize the description
            string key = "{0}\n\n[Click here for more information]({1}).";
            string localizedDescription = string.Format(Localizer.Localize(key, language), present.ContentText[language], $"https://smash.oatmealdome.me/present/{present.Id}/{language.GetCode()}/");

            // Construct the image URL
            string url = $"https://cdn.oatmealdome.me/smash/present/{present.Id}/image.jpg";

            // Construct the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(present.TitleText[language])
                .WithDescription(localizedDescription)
                .AddField(Localizer.Localize("Availability Start Time", language), Localizer.LocalizeDateTime(present.StartDateTime, language), true)
                .AddField(Localizer.Localize("Expiry Time", language), Localizer.LocalizeDateTime(present.EndDateTime, language), true)
                .WithImageUrl(url)
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}