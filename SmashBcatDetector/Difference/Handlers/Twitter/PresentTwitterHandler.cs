using System.Threading.Tasks;
using Nintendo.Bcat;
using SmashBcatDetector.Internationalization;
using SmashBcatDetector.Social;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference.Handlers.Twitter
{
    public class PresentTwitterHandler
    {
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Added, 51)]
        public static void HandleAdded(Present present)
        {
            // Get the tweet header
            string tweetHeaderKey = "[Present]";
            string tweetHeader = Localizer.Localize(tweetHeaderKey, Language.EnglishUS);

            // Get the tweet content
            string tweetContent = present.TitleText[Language.EnglishUS] + "\n\n" + present.ContentText[Language.EnglishUS];

            // Get the tweet URL component
            string tweetUrlKey = "Click here for more information: {0}";
            string tweetUrl = string.Format(Localizer.Localize(tweetUrlKey, Language.EnglishUS), $"https://smash.oatmealdome.me/present/{present.Id}/en-US/");

            // Send the tweet
            TwitterManager.GetAccount("SSBUBot").Tweet(tweetHeader, present.TitleText[Language.EnglishUS], tweetUrl, present.Image);
        }

    }
}