using MessagePack;

namespace Bcat.News
{
    [MessagePackObject]
    public class Version
    {
        [Key("format")]
        public int Format
        {
            get;
            set;
        }

        [Key("semantics")]
        public int Semantics
        {
            get;
            set;
        }

    }
}
