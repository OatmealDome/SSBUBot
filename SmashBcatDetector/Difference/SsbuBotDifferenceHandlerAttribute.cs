using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference
{
    public class SsbuBotDifferenceHandlerAttribute : DifferenceHandlerAttribute
    {
        public SsbuBotDifferenceHandlerAttribute(int type, DifferenceType differenceType, int priority)
            : base(type, differenceType, priority)
        {
            
        }

        public SsbuBotDifferenceHandlerAttribute(FileType type, DifferenceType differenceType, int priority)
            : base((int)type, differenceType, priority)
        {
            
        }
        
    }
}