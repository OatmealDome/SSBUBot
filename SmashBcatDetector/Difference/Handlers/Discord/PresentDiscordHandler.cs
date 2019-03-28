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
    public class PresentDiscordHandler
    {
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Added, 50)]
        public static async Task HandleAdded(Present present)
        {
            // Localize the embed description
            Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("{0}\n\n[Click here for more information]({1}).");

            // Create a new Dictionary for localized descriptions with the URL
            Dictionary<Language, string> localizedDescriptionsWithUrl = new Dictionary<Language, string>();

            // Loop over every localized description
            foreach (KeyValuePair<Language, string> pair in localizedDescriptions)
            {
                // Format the URL in the description
                localizedDescriptionsWithUrl.Add(pair.Key, string.Format(pair.Value, present.ContentText[pair.Key], $"https://smash.oatmealdome.me/present/{present.Id}/{pair.Key.GetCode()}/"));
            }

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = new LocalizedEmbedBuilder()
                .WithTitle(present.TitleText)
                .WithDescription(localizedDescriptionsWithUrl)
                .AddField("Availability Start Time", Localizer.LocalizeDateTimeToAllLanguages(present.StartDateTime))
                .AddField("Expiry Time", Localizer.LocalizeDateTimeToAllLanguages(present.EndDateTime))
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/present/{present.Id}/image.jpg")
                .Build();
            
            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[Present]**", localizedEmbeds);
        }

    }
}