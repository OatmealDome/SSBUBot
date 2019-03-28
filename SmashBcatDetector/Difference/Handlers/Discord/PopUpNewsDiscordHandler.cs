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

namespace SmashBcatDetector.Difference.Handlers.Discord
{
    public class PopUpNewsDiscordHandler
    {
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Added, 50)]
        public static async Task HandleAdded(PopUpNews popUpNews)
        {
            // Localize the embed description
            Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("{0}\n\n[Click here for more information]({1}).");

            // Create a new Dictionary for localized descriptions with the URL
            Dictionary<Language, string> localizedDescriptionsWithUrl = new Dictionary<Language, string>();

            // Loop over every localized description
            foreach (KeyValuePair<Language, string> pair in localizedDescriptions)
            {
                // Format the URL in the description
                localizedDescriptionsWithUrl.Add(pair.Key, string.Format(pair.Value, popUpNews.ContentText[pair.Key], $"https://smash.oatmealdome.me/popup_news/{popUpNews.Id}/{pair.Key.GetCode()}/"));
            }

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = new LocalizedEmbedBuilder()
                .WithTitle(popUpNews.TitleText)
                .WithDescription(localizedDescriptionsWithUrl)
                .AddField("Pop-Up Start Time", Localizer.LocalizeDateTimeToAllLanguages(popUpNews.StartDateTime))
                .AddField("Expiry Time", Localizer.LocalizeDateTimeToAllLanguages(popUpNews.EndDateTime))
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/popup_news/{popUpNews.Id}/image.jpg")
                .Build();
            
            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[News]**", localizedEmbeds);
        }

    }
}