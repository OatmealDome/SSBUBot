using System.Threading.Tasks;
using Nintendo.Bcat;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference.Handlers.Twitter
{
    public class EventTwitterHandler
    {
        /*[SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Added, 51)]
        public static void HandleAdded(Event addedEvent)
        {
            // Get the tweet header
            string tweetHeader = Localizer.Localize("twitter.event.header", Language.EnglishUS);

            // Get the tweet URL component
            string tweetUrl = string.Format(Localizer.Localize("twitter.event.url", Language.EnglishUS), $"https://smash.oatmealdome.me/event/{addedEvent.Id}/en-US/");

            // Send the tweet
            TwitterHandler.Tweet(tweetHeader, addedEvent.TitleText[Language.EnglishUS], tweetUrl, addedEvent.Image);
        }*/

    }
}