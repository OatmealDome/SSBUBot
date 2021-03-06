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
using BcatBotFramework.Difference;

namespace SmashBcatDetector.Difference.Handlers.Discord
{
    public class PopUpNewsDiscordHandler
    {
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Added, 50)]
        public static async Task HandleAdded(PopUpNews popUpNews)
        {
            // Localize the embed description
            Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("popup_news.description", Nintendo.SmashUltimate.Bcat.Container.LanguageOrder);

            // Create a new Dictionary for localized descriptions with the URL
            Dictionary<Language, string> localizedDescriptionsWithUrl = new Dictionary<Language, string>();

            // Loop over every localized description
            foreach (KeyValuePair<Language, string> pair in localizedDescriptions)
            {
                // Format the URL in the description
                localizedDescriptionsWithUrl.Add(pair.Key, string.Format(pair.Value, popUpNews.ContentText[pair.Key], $"https://smash.oatmealdome.me/popup_news/{popUpNews.Id}/{pair.Key.GetCode()}/"));
            }

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = new LocalizedEmbedBuilder(Nintendo.SmashUltimate.Bcat.Container.LanguageOrder)
                .WithTitle(popUpNews.TitleText)
                .WithDescription(localizedDescriptionsWithUrl)
                .AddField("popup_news.start_time", Localizer.LocalizeDateTimeToAllLanguages(popUpNews.StartDateTime))
                .AddField("popup_news.end_time", Localizer.LocalizeDateTimeToAllLanguages(popUpNews.EndDateTime))
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/popup_news/{popUpNews.Id}/image.jpg")
                .Build();
            
            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[News]**", localizedEmbeds);
        }

    }
}