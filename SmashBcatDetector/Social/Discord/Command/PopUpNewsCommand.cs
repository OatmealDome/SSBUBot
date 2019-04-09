using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Core;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.Discord;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class PopUpNewsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("PopUpNews"), Summary("Shows the pop-up news with the specified ID")]
        public async Task Execute(string id = null, string languageString = null)
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context.Guild, languageString);

            // Check for no ID
            if (id == null)
            {
                await ListCommand.ListContainers(FileType.PopUpNews, language, Context);

                return;
            }
            
            // Get the PopUpNews with this ID
            PopUpNews popUpNews = ContainerCache.GetPopUpNewsWithId(id);

            // Check if this exists
            if (popUpNews == null)
            {
                throw new LocalizedException("popup_news.not_found");
            }

            // Localize the description
            string localizedDescription = string.Format(Localizer.Localize("popup_news.description", language), popUpNews.ContentText[language], $"https://smash.oatmealdome.me/popup_news/{popUpNews.Id}/{language.GetCode()}/");

            // Construct the Embed
            Embed embed = new EmbedBuilder()
                .WithTitle(popUpNews.TitleText[language])
                .WithDescription(localizedDescription)
                .AddField(Localizer.Localize("popup_news.start_time", language), Localizer.LocalizeDateTime(popUpNews.StartDateTime, language), true)
                .AddField(Localizer.Localize("popup_news.end_time", language), Localizer.LocalizeDateTime(popUpNews.EndDateTime, language), true)
                .WithImageUrl($"https://cdn.oatmealdome.me/smash/popup_news/{popUpNews.Id}/image.jpg")
                .Build();
            
            await Context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}