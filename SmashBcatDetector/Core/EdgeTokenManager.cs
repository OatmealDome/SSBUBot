using System;
using System.Threading;
using System.Threading.Tasks;
using Nintendo.DAuth;
using BcatBotFramework.Core.Config;
using BcatBotFramework.Core.Config.NintendoCdn;
using SmashBcatDetector.Social;

namespace SmashBcatDetector.Core
{
    public static class EdgeTokenManager
    {
        public static string QLAUNCH_CLIENT_ID = "67bf9945b45248c6"; 

        private static readonly SemaphoreSlim managerSemaphore = new SemaphoreSlim(1, 1);

        public static async Task<string> GetEdgeToken(string clientId)
        {
            // Acquire the semaphore immediately
            await managerSemaphore.WaitAsync();

            // Get the current DateTime
            DateTime dateTime = DateTime.Now;

            // Try to get a CachedToken for the specified client ID
            if (Configuration.LoadedConfiguration.CdnConfig.CachedTokens.TryGetValue(clientId, out CachedToken token))
            {
                if (dateTime < token.ExpiryTime)
                {
                    // Release the semaphore
                    managerSemaphore.Release();

                    // Return the cached token
                    return token.Token;
                }
            }

            // Log that we're getting a new token
            await DiscordBot.LoggingChannel.SendMessageAsync($"**[EdgeTokenManager]** Requesting new token for client ID ``{clientId}``");

            // Request a new challenge
            Challenge challenge = await DAuthApi.GetChallenge();

            // Respond to the challenge
            EdgeToken edgeToken = await DAuthApi.GetEdgeToken(challenge, clientId);

            // Calculate the new expiry time
            dateTime = dateTime.AddSeconds(edgeToken.Expiry);

            // Create a new CachedToken and set fields
            CachedToken cachedToken = new CachedToken();
            cachedToken.Token = edgeToken.Token;
            cachedToken.ExpiryTime = dateTime;

            // Set the CachedToken in the configuration
            Configuration.LoadedConfiguration.CdnConfig.CachedTokens[clientId] = cachedToken;

            // Save the configuration
            Configuration.LoadedConfiguration.Write();

            await DiscordBot.LoggingChannel.SendMessageAsync($"**[EdgeTokenManager]** Success - token expires ``{dateTime.ToString("F")}``");

            // Release the semaphore
            managerSemaphore.Release();

            // Return the token
            return cachedToken.Token;
        }

    }
}