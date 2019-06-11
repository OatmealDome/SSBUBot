#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using SmashBcatDetector.Core;
using BcatBotFramework.Core.Config;
using SmashBcatDetector.Scheduler.Job;
using BcatBotFramework.Social.Discord.Interactive;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Social.Twitter;
using SmashBcatDetector.Difference.Handlers.Twitter;
using System.Linq;
using BcatBotFramework.Json;
using Nintendo.Bcat;
using Nintendo.SmashUltimate.Bcat.LineNewsData;

namespace SmashBcatDetector.Social.Discord.Command
{
    public class DebugCommand : ModuleBase<SocketCommandContext>
    {
        [Command("Debug"), Summary("Debug command")]
        public async Task Execute()
        {
            if (Configuration.LoadedConfiguration.IsProduction)
            {
                return;
            }
            
            /*Event marioEvent = ContainerCache.GetEventWithId(1003);
            string path = Path.Combine(Path.GetDirectoryName(Program.LOCAL_LAST_TOPIC), "debug-event.json");
            File.WriteAllText(path, ToJson(marioEvent));*/
            //await Context.Channel.SendMessageAsync("test <:thonk:508037103473655831>");
            //throw new Exception("test exception");

            //TestMessage testMessage = new TestMessage(Context.User);
            //await DiscordBot.SendInteractiveMessageAsync(Context.Channel, testMessage);

            //RecurringHousekeepingJob job = new RecurringHousekeepingJob();
            //await job.Execute(null);

            //TwitterManager.GetAccount("SSBUBot").Tweet("[Test]", "test tweet from debug command", "URL: https://google.com");
            
            //LineNewsTwitterHandler.HandleAdded(ContainerCache.GetLineNews().Last());

            /*IList<PopUpNews> newsList = ContainerCache.GetPopUpNews();
            PopUpNews longestNews = newsList.FirstOrDefault();
            foreach (PopUpNews news in newsList)
            {
                if (news.TitleText[Language.EnglishUS].Length > longestNews.TitleText[Language.EnglishUS].Length)
                {
                    longestNews = news;
                }
            }

            await Context.Channel.SendMessageAsync("[Debug] tweeting " + longestNews.Id);

            PopUpNewsTwitterHandler.HandleAdded(longestNews);*/

            /*IList<LineNews> newsList = ContainerCache.GetLineNews();

            LineNews longestNews = newsList.FirstOrDefault();
            OneLine longestLine = longestNews.OneLines[0];

            foreach (LineNews news in newsList)
            {
                foreach (OneLine line in news.OneLines)
                {
                    if (line.Text[Language.EnglishUS].Length > longestLine.Text[Language.EnglishUS].Length)
                    {
                        longestNews = news;
                        longestLine = line;
                    }
                }
            }

            await Context.Channel.SendMessageAsync("[Debug] line news " + longestNews.Id + ", oneline " + longestLine.Id + " is " + longestLine.Text[Language.EnglishUS].Length);*/

            await Context.Channel.SendMessageAsync("[Debug] Test tweets...");

            PopUpNewsTwitterHandler.HandleAdded(ContainerCache.GetPopUpNews().FirstOrDefault());
            PresentTwitterHandler.HandleAdded(ContainerCache.GetPresents().FirstOrDefault());
            LineNewsTwitterHandler.HandleAdded(ContainerCache.GetLineNews().FirstOrDefault());
        }

        private static string ToJson(Nintendo.SmashUltimate.Bcat.Container container)
        {
            return JsonConvert.SerializeObject(container, new JsonSerializerSettings()
            {
                ContractResolver = new ShouldSerializeContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new LanguageMappingDictionaryConverter()
                },
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
        }
    }

}

#endif