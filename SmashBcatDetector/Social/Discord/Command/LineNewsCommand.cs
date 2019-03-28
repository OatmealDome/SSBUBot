using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Core;
using BcatBotFramework.Internationalization;
using SmashBcatDetector.Util;
using Nintendo.SmashUltimate.Bcat;
using Nintendo.SmashUltimate.Bcat.LineNewsData;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class LineNewsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("LineNews"), Summary("Shows the line news with the specified ID")]
        public async Task Execute(string id = null, string languageString = null)
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, languageString);

            // Check for no ID
            if (id == null)
            {
                await ListCommand.ListContainers(FileType.LineNews, language, Context);

                return;
            }
            
            // Get the PopUpNews with this ID
            LineNews lineNews = ContainerCache.GetLineNewsWithId(id);

            // Check if this exists
            if (lineNews == null)
            {
                throw new LocalizedException("No line news exists with this ID");
            }

            // Localize the title
            string titleKey = "Line News ({0})";
            string localizedTitle = string.Format(Localizer.Localize(titleKey, language), lineNews.Id);

            // Localize the description
            string descriptionKey = "[Click here for more information]({0}).";
            string localizedDescription = string.Format(Localizer.Localize(descriptionKey, language), $"https://smash.oatmealdome.me/line_news/{lineNews.Id}/{language.GetCode()}/");

            // Localize the line field name
            string lineFieldNameKey = "Line {0}";
            string localizedLineFieldName = Localizer.Localize(lineFieldNameKey, language);

            // Construct the Embed
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(localizedTitle)
                //.WithDescription(localizedDescription)
                .AddField(Localizer.Localize("Start Time", language), Localizer.LocalizeDateTime(lineNews.StartDateTime, language), true)
                .AddField(Localizer.Localize("Expiry Time", language), Localizer.LocalizeDateTime(lineNews.EndDateTime, language), true);

            // Add every OneLine
            foreach (OneLine oneLine in lineNews.OneLines)
            {
                embedBuilder.AddField(string.Format(localizedLineFieldName, oneLine.Id), oneLine.Text[language]);
            }

            await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }
        
    }
}