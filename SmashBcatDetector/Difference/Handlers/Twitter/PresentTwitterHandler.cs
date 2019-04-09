using System.Threading.Tasks;
using Nintendo.Bcat;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Difference;
using BcatBotFramework.Social.Twitter;

namespace SmashBcatDetector.Difference.Handlers.Twitter
{
    public class PresentTwitterHandler
    {
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Added, 51)]
        public static void HandleAdded(Present present)
        {
            // Get the tweet header
            string tweetHeader = Localizer.Localize("twitter.present.header", Language.EnglishUS);

            // Get the tweet content
            string tweetContent = present.TitleText[Language.EnglishUS] + "\n\n" + present.ContentText[Language.EnglishUS];

            // Get the tweet URL component
            string tweetUrl = string.Format(Localizer.Localize("twitter.present.url", Language.EnglishUS), $"https://smash.oatmealdome.me/present/{present.Id}/en-US/");

            // Send the tweet
            TwitterManager.GetAccount("SSBUBot").Tweet(tweetHeader, present.TitleText[Language.EnglishUS], tweetUrl, present.Image);
        }

    }
}