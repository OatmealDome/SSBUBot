using System.Threading.Tasks;
using Nintendo.Bcat;
using SmashBcatDetector.Internationalization;
using SmashBcatDetector.Social;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference.Handlers.Twitter
{
    public class EventTwitterHandler
    {
        /*[DifferenceHandler((int)FileType.Event, DifferenceType.Added, 51)]
        public static void HandleAdded(Event addedEvent)
        {
            // Get the tweet header
            string tweetHeaderKey = "[Event]";
            string tweetHeader = Localizer.Localize(tweetHeaderKey, Language.EnglishUS);

            // Get the tweet URL component
            string tweetUrlKey = "Click here for more information: {0}";
            string tweetUrl = string.Format(Localizer.Localize(tweetUrlKey, Language.EnglishUS), $"https://smash.oatmealdome.me/event/{addedEvent.Id}/en-US/");

            // Send the tweet
            TwitterHandler.Tweet(tweetHeader, addedEvent.TitleText[Language.EnglishUS], tweetUrl, addedEvent.Image);
        }*/

    }
}