﻿using MessagePack;

namespace Bcat.News
{
    [MessagePackObject]
    public class Subject
    {
        [Key("caption")]
        public int Caption
        {
            get;
            set;
        }

        [Key("text")]
        public string Text
        {
            get;
            set;
        }

    }
}
