using System.Threading.Tasks;
using Nintendo.Bcat;
using BcatBotFramework.Internationalization;
using BcatBotFramework.Social.Discord;
using Nintendo.SmashUltimate.Bcat;
using BcatBotFramework.Difference;
using BcatBotFramework.Social.Twitter;

namespace SmashBcatDetector.Difference.Handlers.Twitter
{
    public class PopUpNewsTwitterHandler
    {
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Added, 51)]
        public static void HandleAdded(PopUpNews popUpNews)
        {
            // Check if this is an event pop-up news and is not the initial pop-up
            if (popUpNews.IsPopUpForEvent && !popUpNews.Id.StartsWith("01"))
            {
                // Skip
                return;
            }
            
            // Get the tweet header
            string tweetHeader = Localizer.Localize("twitter.popupnews.header", Language.EnglishUS);

            // Get the tweet URL component
            string tweetUrl = string.Format(Localizer.Localize("twitter.popupnews.url", Language.EnglishUS), $"https://smash.oatmealdome.me/popup_news/{popUpNews.Id}/en-US/");

            // Send the tweet
            TwitterManager.GetAccount("SSBUBot").Tweet(tweetHeader, popUpNews.TitleText[Language.EnglishUS], tweetUrl, popUpNews.Image);
        }

    }
}