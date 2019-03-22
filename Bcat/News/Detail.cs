using System.Collections.Generic;
using Bcat.News.Catalog;
using MessagePack;

namespace Bcat.News
{
    [MessagePackObject]
    public class Detail : Entry
    {
        [Key("latest_news_urls")]
        public List<string> LatestNewsUrls
        {
            get;
            set;
        }
        
    }
}