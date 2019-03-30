using BcatBotFramework.Scheduler.Job;
using SmashBcatDetector.Core;

namespace SmashBcatDetector.Scheduler.Job
{
    public class SsbuBotShutdownJob : ShutdownJob
    {
        protected override void ShutdownAppSpecificItems()
        {
            // Shutdown the ContainerCache
            ContainerCache.Dispose();
        }

    }
}