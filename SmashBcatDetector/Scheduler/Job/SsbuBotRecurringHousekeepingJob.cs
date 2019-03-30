using System.Threading.Tasks;

namespace SmashBcatDetector.Scheduler.Job
{
    public class SsbuBotRecurringHousekeepingJob : RecurringHousekeepingJob
    {
        protected override Task RunAppSpecificRecurringTasks()
        {
            // Nothing for now
            return Task.FromResult(0);
        }

    }
}