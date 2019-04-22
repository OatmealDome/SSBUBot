using BcatBotFramework.Core;
using SmashBcatDetector.Core;

namespace SmashBcatDetector.Scheduler.Job
{
    public class SsbuBotShutdown : Shutdown
    {
        protected override void ShutdownAppSpecificItems()
        {
            // Shutdown the ContainerCache
            ContainerCache.Dispose();
        }

    }
}