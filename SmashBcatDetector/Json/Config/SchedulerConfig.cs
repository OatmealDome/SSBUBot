using Newtonsoft.Json;
using SmashBcatDetector.Json.Config.Scheduler;

namespace SmashBcatDetector.Json.Config
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