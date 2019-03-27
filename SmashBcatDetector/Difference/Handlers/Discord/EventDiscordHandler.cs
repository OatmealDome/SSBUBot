using System.Collections.Generic;
using System.IO;
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
    public class EventDiscordHandler
    {
        [DifferenceHandler(FileType.Event, DifferenceType.Added, 50)]
        public static async Task HandleAdded(Event addedEvent)
        {
            // Localize the embed description
            Dictionary<Language, string> localizedDescriptions = Localizer.LocalizeToAllLanguages("[Click here for more information]({0}).");

            // Create a new Dictionary for localized descriptions with the URL
            Dictionary<Language, string> localizedDescriptionsWithUrl = new Dictionary<Language, string>();

            // Loop over every localized description
            foreach (KeyValuePair<Language, string> pair in localizedDescriptions)
            {
                // Format the URL in the description
                localizedDescriptionsWithUrl.Add(pair.Key, string.Format(pair.Value, $"https://smash.oatmealdome.me/event/{addedEvent.Id}/{pair.Key.GetCode()}/"));
            }

            // Get the localized time field names
            Dictionary<Language, string> startFieldName = Localizer.LocalizeToAllLanguages("Start Time");
            Dictionary<Language, string> endFieldName = Localizer.LocalizeToAllLanguages("End Time");

            // Create localized Embeds
            Dictionary<Language, Embed> localizedEmbeds = new LocalizedEmbedBuilder()
                .WithTitle(addedEvent.TitleText)
                .WithDescription(localizedDescriptionsWithUrl)
                .AddField("Start Time", Localizer.LocalizeDateTimeToAllLanguages(addedEvent.StartDateTime))
                .AddField("End Time", Localizer.LocalizeDateTimeToAllLanguages(addedEvent.EndDateTime))
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/event/{addedEvent.Id}/image.jpg")
                .Build();
            
            // Send the notifications
            await DiscordBot.SendNotificationAsync("**[Event]**", localizedEmbeds);
        }

    }
}