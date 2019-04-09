using System.Collections.Generic;
using System.IO;
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
    public class EventDiscordHandler
    {
        [SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Added, 50)]
        public static async Task HandleAdded(Event addedEvent)
        {
            // Localize the embed description
            Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("event.more_info", Nintendo.SmashUltimate.Bcat.Container.LanguageOrder);

            // Create a new Dictionary for localized descriptions with the URL
            Dictionary<Language, string> localizedDescriptionsWithUrl = new Dictionary<Language, string>();

            // Loop over every localized description
            foreach (KeyValuePair<Language, string> pair in localizedDescriptions)
            {
                // Format the URL in the description
                localizedDescriptionsWithUrl.Add(pair.Key, string.Format(pair.Value, $"https://smash.oatmealdome.me/event/{addedEvent.Id}/{pair.Key.GetCode()}/"));
            }

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = new LocalizedEmbedBuilder(Nintendo.SmashUltimate.Bcat.Container.LanguageOrder)
                .WithTitle(addedEvent.TitleText)
                .WithDescription(localizedDescriptionsWithUrl)
                .AddField("event.start_time", Localizer.LocalizeDateTimeToAllLanguages(addedEvent.StartDateTime))
                .AddField("event.end_time", Localizer.LocalizeDateTimeToAllLanguages(addedEvent.EndDateTime))
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/event/{addedEvent.Id}/image.jpg")
                .Build();
            
            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[Event]**", localizedEmbeds);
        }

    }
}