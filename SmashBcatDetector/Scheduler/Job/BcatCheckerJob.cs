using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Discord.WebSocket;
using MessagePack;
using Quartz;
using SmashBcatDetector.Core;
using SmashBcatDetector.Difference;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Social.Discord;
using SmashBcatDetector.Util;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Difference;
using BcatBotFramework.Core;

namespace SmashBcatDetector.Scheduler.Job
{
    public class BcatCheckerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Log that we're about to begin a check
                await DiscordBot.LoggingChannel.SendMessageAsync("**[BCAT]** Beginning check");

                // Download the latest Topic
                Topic topic = await BcatApi.GetDataTopic(Program.TOPIC_ID, Program.TITLE_ID, Program.PASSPHRASE);

                // Create the target folder name
                string targetFolder = string.Format(Program.LOCAL_OLD_DATA_DIRECTORY, DateTime.Now.ToString(Program.FOLDER_DATE_TIME_FORMAT));

                // Check if this the first run
                if (!Configuration.LoadedConfiguration.FirstRunCompleted)
                {
                    // Log that this is the first run of BCAT
                    await DiscordBot.LoggingChannel.SendMessageAsync("**[BCAT]** First run");

                    // Download all data
                    Dictionary<string, byte[]> downloadedData = await BcatCheckerUtils.DownloadAllData(topic, Program.TITLE_ID, Program.PASSPHRASE, targetFolder);

                    // Loop over all data
                    foreach (KeyValuePair<string, byte[]> pair in downloadedData)
                    {
                        // Get the FileType
                        FileType fileType = FileTypeExtensions.GetTypeFromName(pair.Key);

                        // Check if this is a container
                        if (fileType.IsContainer())
                        {
                            // Create a Container instance
                            Nintendo.SmashUltimate.Bcat.Container container = CreateSmashContainerInstance(fileType, pair.Value);

                            // Add this to the container cache
                            ContainerCache.AddFile(container, Path.GetFileName(pair.Key), pair.Value);
                        }
                        else
                        {
                            // Write this file out to the common file cache
                            File.WriteAllBytes(Path.Combine(Program.LOCAL_COMMON_CACHE_DIRECTORY, Path.GetFileName(pair.Key)), pair.Value);
                        }
                    }

                    // Save the configuration
                    Configuration.LoadedConfiguration.FirstRunCompleted = true;
                    Configuration.LoadedConfiguration.Write();

                    // Write out the topic
                    File.WriteAllBytes(Program.LOCAL_LAST_TOPIC, MessagePackSerializer.Serialize(topic));

                    // Log that we're about to begin a check
                    await DiscordBot.LoggingChannel.SendMessageAsync("**[BCAT]** First run complete");

                    return;
                }

                // Load the old Topic
                Topic oldTopic = MessagePackSerializer.Deserialize<Topic>(File.ReadAllBytes(Program.LOCAL_LAST_TOPIC));

#if DEBUG
                if (!Configuration.LoadedConfiguration.IsProduction)
                {
                    /*foreach (Bcat.Directory dir in oldTopic.Directories)
                    {
                        if (dir.Name == "line_news")
                        {
                            Data dbgData = dir.Data.FirstOrDefault();
                            if (dbgData == null)
                            {
                                continue;
                            }
                            dir.Data.Remove(dbgData);
                            //dbgData.Digest = "deadbeef";
                        }
                    }*/
                }
#endif

                // Get the differences
                List<KeyValuePair<DifferenceType, string>> differences = BcatCheckerUtils.GetTopicChanges(oldTopic, topic);

                // Check if there aren't any
                if (differences.Count == 0)
                {
                    // Nothing to do here
                    goto finished;
                }

                // Download all data
                Dictionary<string, byte[]> data = await BcatCheckerUtils.DownloadAllData(topic, Program.TITLE_ID, Program.PASSPHRASE, targetFolder);

                // Loop over every difference
                foreach (KeyValuePair<DifferenceType, string> pair in differences)
                {
                    // Log the difference
                    await DiscordBot.LoggingChannel.SendMessageAsync("**[BCAT]** diff: ``" + pair.Value + "`` (" + pair.Key.ToString() + ")");

                    // Get the FileType
                    FileType fileType = FileTypeExtensions.GetTypeFromName(pair.Value);

                    // Declare a variable to hold the method parameters
                    object[] parameters;

                    if (pair.Key == DifferenceType.Added)
                    {
                        // Get the raw file
                        byte[] rawFile = data[pair.Value];

                        // Check if this is a Container
                        if (fileType.IsContainer())
                        {
                            // Create a Container instance
                            Nintendo.SmashUltimate.Bcat.Container addedContainer = CreateSmashContainerInstance(fileType, data[pair.Value]);

                            // Add this to the container cache
                            ContainerCache.AddFile(addedContainer, Path.GetFileName(pair.Value), rawFile);

                            // Set the method parameters
                            parameters = new object[] { addedContainer };
                        }
                        else
                        {
                            // Write this file out to the common file cache
                            File.WriteAllBytes(Path.Combine(Program.LOCAL_COMMON_CACHE_DIRECTORY, Path.GetFileName(pair.Value)), data[pair.Value]);

                            // Set the method parameters
                            parameters = new object[] { rawFile };
                        }
                    }
                    else if (pair.Key == DifferenceType.Changed)
                    {
                        // Get the raw file
                        byte[] rawFile = data[pair.Value];

                        // Check if this is a Container
                        if (fileType.IsContainer())
                        {
                            // Create a Container instance
                            Nintendo.SmashUltimate.Bcat.Container addedContainer = CreateSmashContainerInstance(fileType, data[pair.Value]);

                            // Overwrite this to the container cache
                            Nintendo.SmashUltimate.Bcat.Container previousContainer = ContainerCache.OverwriteFile(addedContainer, Path.GetFileName(pair.Value), rawFile);

                            // Set the method parameters
                            parameters = new object[] { previousContainer, addedContainer };
                        }
                        else
                        {
                            // Construct the commoon cache path
                            string path = Path.Combine(Program.LOCAL_COMMON_CACHE_DIRECTORY, Path.GetFileName(pair.Value));

                            // Load the old file
                            byte[] previousRawFile = File.ReadAllBytes(path);

                            // Write this file out to the common file cache
                            File.WriteAllBytes(path, data[pair.Value]);

                            // Set the method parameters
                            parameters = new object[] { previousRawFile, rawFile };
                        }
                    }
                    else // Removed
                    {
                        // TODO: discord print
                        continue;
                    }

                    // Call every difference handler
                    await BcatCheckerUtils.CallDifferenceHandlers((int)fileType, pair.Key, parameters);
                }

                // Write out the Topic
                File.WriteAllBytes(Program.LOCAL_LAST_TOPIC, MessagePackSerializer.Serialize(topic));

    finished:
                await DiscordBot.LoggingChannel.SendMessageAsync("**[BCAT]** Check complete");
            }
            catch (Exception exception)
            {
                // Notify the logging channel
                await DiscordUtil.HandleException(exception, $"in ``BcatCheckerJob``");
            }
        }

        private Nintendo.SmashUltimate.Bcat.Container CreateSmashContainerInstance(FileType fileType, byte[] rawFile)
        {
            // Load the file into a stream
            using (MemoryStream memoryStream = new MemoryStream(rawFile))
            {
                switch (fileType)
                {
                    case FileType.Event:
                        return new Event(memoryStream);
                    case FileType.LineNews:
                        return new LineNews(memoryStream);
                    case FileType.PopUpNews:
                        return new PopUpNews(memoryStream);
                    case FileType.Present:
                        return new Present(memoryStream);
                    default:
                        throw new Exception("Not a Container");
                }
            }
        }

    }
}