using MessagePack;

namespace Bcat.News
{
    [MessagePackObject]
    public class Footer
    {
        [Key("text")]
        public string Text
        {
            get;
            set;
        }

    }
}
