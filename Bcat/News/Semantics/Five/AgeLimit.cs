using MessagePack;
using System.Collections.Generic;

namespace Bcat.News.Semantics.Five
{
    [MessagePackObject]
    public class AgeLimit
    {
        [Key("ratings")]
        public List<Rating> Ratings
        {
            get;
            set;
        }

    }
}
