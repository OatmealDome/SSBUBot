using System.Threading.Tasks;
using Discord.WebSocket;
using Quartz;
using SmashBcatDetector.Social;
using SmashBcatDetector.Social.Discord;

namespace SmashBcatDetector.Scheduler.Job
{
    public class DiscordPlayingAlternatorJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // Increment the current state
            DiscordBot.PlayingState++;

            // Check if we need to loop
            if (DiscordBot.PlayingState == PlayingState.LoopToBeginning)
            {
                DiscordBot.PlayingState = PlayingState.Smash;
            }

            // Set the correct text
            string text;
            switch (DiscordBot.PlayingState)
            {
                case PlayingState.Smash:
                    text = "Super Smash Bros. Ultimate";
                    break;
                case PlayingState.UserCount:
                    // Count the total number of users
                    int users = 0;
                    foreach (SocketGuild guild in DiscordBot.GetGuilds())
                    {
                        users += guild.MemberCount;
                    }

                    text = $"with {users} users | ssbu.help";
                    break;
                case PlayingState.Help:
                    text = "command list | ssbu.help";
                    break;
                case PlayingState.ServerCount:
                    text = $"in {DiscordBot.GetGuilds().Count} servers | ssbu.help";
                    break;
                case PlayingState.Invite:
                    text = "invite me | ssbu.invite";
                    break;
                default:
                    text = "Error";
                    break;
            }

            // Set the text
            await DiscordBot.SetGameAsync(text);
        }

    }
}