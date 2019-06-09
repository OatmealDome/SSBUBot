using System.Threading.Tasks;
using Nintendo.Bcat;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Difference;
using BcatBotFramework.Social.Twitter;

namespace SmashBcatDetector.Difference.Handlers.Twitter
{
    public class LineNewsTwitterHandler
    {
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Added, 51)]
        public static void HandleAdded(LineNews lineNews)
        {
            // Get the tweet header
            string tweetHeader = Localizer.Localize("twitter.line_news.header", Language.EnglishUS);

            // Get the tweet content
            string tweetContent = $"\"{lineNews.OneLines[0].Text[Language.EnglishUS]}\" and more...";

            // Get the tweet URL component
            string tweetUrl = string.Format(Localizer.Localize("twitter.line_news.url", Language.EnglishUS), $"https://smash.oatmealdome.me/line_news/{lineNews.Id}/en-US/");

            // Send the tweet
            TwitterManager.GetAccount("SSBUBot").Tweet(tweetHeader, tweetContent, tweetUrl);
        }

    }
}