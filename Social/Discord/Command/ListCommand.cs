using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bcat;
using Discord;
using Discord.Commands;
using SmashBcatDetector.Core;
using SmashBcatDetector.Util;
using SmashUltimate.Bcat;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class ListCommand : ModuleBase<SocketCommandContext>
    {
        [Command("list"), Summary("Lists all data with the specified type.")]
        public async Task Execute(string containerType, string specifiedLanguage = null)
        {
            // Get the language
            Language language = DiscordUtil.GetDefaultLanguage(Context, specifiedLanguage);

            // Get the FileType
            FileType fileType;
            if (containerType == "linenews")
            {
                fileType = FileType.LineNews;
            }
            if (containerType == "popupnews")
            {
                fileType = FileType.PopUpNews;
            }
            else if (containerType == "present")
            {
                fileType = FileType.Present;
            }
            else if (containerType == "event")
            {
                fileType = FileType.Event;
            }
            else
            {
                throw new LocalizedException("Unknown file type");    
            }

            // Get the list
            await ListContainers(fileType, language, Context);
        }

        public static async Task ListContainers(FileType fileType, Language language, SocketCommandContext context)
        {
            // Create a list of IDs and their names
            List<string> ids = new List<string>();

            switch (fileType)
            {
                case FileType.Event:
                    foreach (Event smashEvent in ContainerCache.GetEvents())
                    {
                        ids.Add($"``{smashEvent.Id}`` - {smashEvent.TitleText[language]}");
                    }

                    break;
                case FileType.LineNews:
                    foreach (LineNews lineNews in ContainerCache.GetLineNews())
                    {
                        ids.Add($"``{lineNews.Id}``");
                    }
                    
                    break;
                case FileType.PopUpNews:
                    foreach (PopUpNews news in ContainerCache.GetPopUpNews())
                    {
                        ids.Add($"``{news.Id}`` - {news.TitleText[language]}");
                    }

                    break;
                case FileType.Present:
                    foreach (Present present in ContainerCache.GetPresents())
                    {
                        ids.Add($"``{present.Id}`` - {present.TitleText[language]}");
                    }

                    break;
                default:
                    throw new LocalizedException("Unsupported file type");
            }

            Embed embed = new EmbedBuilder()
                .WithTitle("List")
                .WithDescription("Here are the valid IDs:\n" + string.Join('\n', ids) + "")
                .WithColor(Color.Green)
                .Build();
            
            await context.Channel.SendMessageAsync(embed: embed);
        }
        
    }
}