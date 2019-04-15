using System.Threading.Tasks;
using BcatBotFramework.Core;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using BcatBotFramework.Social.Discord;
using Nintendo.Bcat;

namespace SmashBcatDetector.Core.OneTime
{
    public class LanguageConversionOneTimeTask : OneTimeTask
    {
        protected override async Task Run()
        {
            await DiscordBot.LoggingChannel.SendMessageAsync($"**[LanguageConversionOneTimeTask]** Converting languages");

            // Set the logging channel language
            Configuration.LoadedConfiguration.DiscordConfig.LoggingTargetChannel.DefaultLanguage = Language.EnglishUS;

            // Convert all GuildSettings
            foreach (GuildSettings guildSettings in Configuration.LoadedConfiguration.DiscordConfig.GuildSettings)
            {
                guildSettings.DefaultLanguage = guildSettings.DefaultLanguage - 2;
            }
        }

    }
}