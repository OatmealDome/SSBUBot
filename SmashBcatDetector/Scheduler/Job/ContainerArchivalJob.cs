using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nintendo.Bcat;
using Quartz;
using S3;
using SmashBcatDetector.Social;
using SmashBcatDetector.Util;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Scheduler.Job
{
    public class ContainerArchivalJob : IJob
    {
        public static string ARCHIVAL_DATA_PREFIX = "namePrefix";
        public static string ARCHIVAL_DATA_ID = "containerId";
        public static string ARCHIVAL_DATA_URL = "unformattedUrl";

        private static SocketsHttpHandler socketsHttpHandler;
        private static HttpClient httpClient;
        private static string ARCHIVAL_USER_AGENT = $"Mozilla/5.0 (compatible; SSBUBot/1.0; +https://smash.oatmealdome.me/for-nintendo/)";
        
        static ContainerArchivalJob()
        {
            socketsHttpHandler = new SocketsHttpHandler();
            socketsHttpHandler.PooledConnectionLifetime = new TimeSpan(0, 10, 0);

            httpClient = new HttpClient(socketsHttpHandler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", ARCHIVAL_USER_AGENT);
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            // Get the values from the JobDataMap
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string namePrefix = dataMap.GetString(ARCHIVAL_DATA_PREFIX);
            string id = dataMap.GetString(ARCHIVAL_DATA_ID);
            string unformattedUrl = dataMap.GetString(ARCHIVAL_DATA_URL);

            // Format the S3 path
            string s3Path = $"/smash/{namePrefix}/{id}/page";

            try
            {
                // Loop over every Language
                foreach (Language language in Nintendo.SmashUltimate.Bcat.Container.LanguageOrder)
                {
                    // Get the URL for this language
                    string url = unformattedUrl.Replace("{$lang}", language.GetCode());

                    // Perform an HTTP request
                    using (HttpResponseMessage response = await httpClient.GetAsync(url))
                    using (HttpContent content = response.Content)
                    {
                        // Check for a success
                        if (!response.IsSuccessStatusCode)
                        {
                            // Check for 404
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                // Log
                                await DiscordBot.LoggingChannel.SendMessageAsync($"**[ArchivalJob]** {language} for {namePrefix}_{id} not found");

                                continue;
                            }

                            // Unknown error
                            throw new Exception("Archival failure on HTTP request (" + response.StatusCode + ")");
                        }
                        
                        // Get the content
                        byte[] rawContent = await content.ReadAsByteArrayAsync();

                        // Write the data to S3
                        S3Api.TransferFile(rawContent, s3Path, language.GetCode() + ".html");

                        await DiscordBot.LoggingChannel.SendMessageAsync($"**[ArchivalJob]** Downloaded {language} for {namePrefix}_{id}");
                    }
                }
            }
            catch (Exception exception)
            {
                // Notify the logging channel
                await DiscordUtil.HandleException(exception, $"in ``ContainerArchivalJob``");
            }
        }

    }
}