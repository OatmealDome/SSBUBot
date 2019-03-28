using System.Collections.Generic;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.WebSocket;
using SmashBcatDetector.Internationalization;
using SmashBcatDetector.Internationalization.Discord;
using SmashBcatDetector.Core.Config;
using SmashBcatDetector.Core.Config.Discord;
using SmashBcatDetector.Social;
using Nintendo.SmashUltimate.Bcat;
using Nintendo.SmashUltimate.Bcat.LineNewsData;

namespace SmashBcatDetector.Difference.Handlers.Discord
{
    public class LineNewsDiscordHandler
    {
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Added, 50)]
        public static async Task HandleAdded(LineNews lineNews)
        {
            // Localize the embed title
            Dictionary<Language, string> localizedTitles = Localizer.LocalizeToAllLanguagesWithFormat("Line News ({0})", lineNews.Id);

            // Localize the embed description
            // Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("[Click here for more information]({0}).", $"https://smash.oatmealdome.me/line_news/{lineNews.Id}/{pair.Key.GetCode()}/");

            // Create localized Embeds
            LocalizedEmbedBuilder localizedEmbedBuilder = new LocalizedEmbedBuilder()
                .WithTitle(localizedTitles)
                //.WithDescription(localizedDescriptions)
                .AddField("Start Time", Localizer.LocalizeDateTimeToAllLanguages(lineNews.StartDateTime))
                .AddField("Expiry Time", Localizer.LocalizeDateTimeToAllLanguages(lineNews.EndDateTime));

            // Add every OneLine
            foreach (OneLine oneLine in lineNews.OneLines)
            {
                localizedEmbedBuilder.AddField(Localizer.LocalizeToAllLanguagesWithFormat("Line {0}", oneLine.Id), oneLine.Text);
            }

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = localizedEmbedBuilder.Build();

            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[Line News]**", localizedEmbeds);
        }

    }
}