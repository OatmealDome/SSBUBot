using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord.WebSocket;
using SmashBcatDetector.Internationalization;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using SmashBcatDetector.Social;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference.Handlers.Discord
{
    public class GenericChangedDiscordHandler
    {
        // TODO: Embed-ify
        
        /*[SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Changed, 100)]
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Changed, 100)]
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Changed, 100)]
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Changed, 100)]
        public static async Task HandleChanged(Nintendo.SmashUltimate.Bcat.Container previousContainer, Nintendo.SmashUltimate.Bcat.Container newContainer)
        {
            // Localize the changed alert text
            string alertTextKey = "**[Change]** The data for the {0} with ID {1} was modified.";
            Dictionary<Language, string> alertText = Localizer.LocalizeToAllLanguages(alertTextKey);

            // Format the text
            Dictionary<Language, string> localizedAlertText = new Dictionary<Language, string>();

            // Loop over every localized description
            foreach (KeyValuePair<Language, string> pair in alertText)
            {
                // Format the URL in the description
                localizedAlertText.Add(pair.Key, string.Format(pair.Value, newContainer.GetType().Name, newContainer.Id));
            }

            // Loop over every TargetChannel
            foreach (GuildSettings guildSettings in Configuration.LoadedConfiguration.DiscordConfig.GuildSettings)
            {
                // Get the channel
                ISocketMessageChannel channel = DiscordBot.GetChannel(guildSettings);

                // Send the message
                await channel.SendMessageAsync(localizedAlertText[guildSettings.DefaultLanguage]);
            }
        }*/

    }
}