using SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class DifferenceHandlerAttribute : System.Attribute
    {
        public FileType FileType
        {
            get;
            set;
        }

        public DifferenceType DifferenceType
        {
            get;
            set;
        }

        public int Priority
        {
            get;
            set;
        }
        
        public DifferenceHandlerAttribute(FileType fileType, DifferenceType differenceType, int priority)
        {
            // Set fields
            this.FileType = fileType;
            this.DifferenceType = differenceType;
            this.Priority = priority;
        }

    }
}