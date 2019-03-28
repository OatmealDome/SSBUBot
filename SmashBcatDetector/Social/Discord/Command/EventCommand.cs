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
using SmashBcatDetector.Util;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class EventCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Event"), Summary("Shows the event with the specified ID")]
        public async Task Execute(string id = null, string languageString = null)
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, languageString);

            // Check for no ID
            if (id == null)
            {
                await ListCommand.ListContainers(FileType.Event, language, Context);

                return;
            }

            // Get the PopUpNews with this ID
            Event smashEvent = ContainerCache.GetEventWithId(id);

            // Check if this exists
            if (smashEvent == null)
            {
                throw new LocalizedException("No event exists with this ID");
            }

            // Localize the description
            string key = "[Click here for more information]({0}).";
            string localizedDescription = string.Format(Localizer.Localize(key, language), $"https://smash.oatmealdome.me/event/{smashEvent.Id}/{language.GetCode()}/");

            // Construct the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(smashEvent.TitleText[language])
                .WithDescription(localizedDescription)
                .AddField(Localizer.Localize("Start Time", language), Localizer.LocalizeDateTime(smashEvent.StartDateTime, language), true)
                .AddField(Localizer.Localize("End Time", language), Localizer.LocalizeDateTime(smashEvent.EndDateTime, language), true)
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/event/{smashEvent.Id}/image.jpg")
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}