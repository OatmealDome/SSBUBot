using System.Collections.Generic;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.WebSocket;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Internationalization.Discord;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using Nintendo.SmashUltimate.Bcat.LineNewsData;
using BcatBotFramework.Difference;

namespace SmashBcatDetector.Difference.Handlers.Discord
{
    public class LineNewsDiscordHandler
    {
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Added, 50)]
        public static async Task HandleAdded(LineNews lineNews)
        {
            // Localize the embed title
            Dictionary<Language, string> localizedTitles = Localizer.LocalizeToAllLanguagesWithFormat("line_news.title", Nintendo.SmashUltimate.Bcat.Container.LanguageOrder, lineNews.Id);

            // Localize the embed description
            // Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("line_news.more_info", $"https://smash.oatmealdome.me/line_news/{lineNews.Id}/{pair.Key.GetCode()}/");

            // Create localized Embeds
            LocalizedEmbedBuilder localizedEmbedBuilder = new LocalizedEmbedBuilder(Nintendo.SmashUltimate.Bcat.Container.LanguageOrder)
                .WithTitle(localizedTitles)
                //.WithDescription(localizedDescriptions)
                .AddField("line_news.start_time", Localizer.LocalizeDateTimeToAllLanguages(lineNews.StartDateTime))
                .AddField("line_news.end_time", Localizer.LocalizeDateTimeToAllLanguages(lineNews.EndDateTime));

            // Add every OneLine
            foreach (OneLine oneLine in lineNews.OneLines)
            {
                localizedEmbedBuilder.AddField(Localizer.LocalizeToAllLanguagesWithFormat("line_news.line_title", oneLine.Id), oneLine.Text);
            }

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = localizedEmbedBuilder.Build();

            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[Line News]**", localizedEmbeds);
        }

    }
}