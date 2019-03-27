using Newtonsoft.Json;
using SmashBcatDetector.Core.Config.Scheduler;

namespace SmashBcatDetector.Core.Config
{
    public class SchedulerConfig
    {
        [JsonProperty("Bcat")]
        public JobSchedule BcatJob
        {
            get;
            set;
        }

        [JsonProperty("Housekeeping")]
        public JobSchedule HousekeepingJob
        {
            get;
            set;
        }

    }
}