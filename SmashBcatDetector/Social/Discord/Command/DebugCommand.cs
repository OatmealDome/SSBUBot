#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using SmashBcatDetector.Core;
using SmashBcatDetector.Json;
using SmashBcatDetector.Json.Config;
using SmashBcatDetector.Scheduler.Job;
using SmashBcatDetector.Social.Discord.Interactive;
using Nintendo.SmashUltimate.Bcat;

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

            TestMessage testMessage = new TestMessage();
            await DiscordBot.SendInteractiveMessageAsync(Context.Channel, testMessage);

            //RecurringHousekeepingJob job = new RecurringHousekeepingJob();
            //await job.Execute(null);
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