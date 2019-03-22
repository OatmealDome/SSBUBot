using System;

namespace SmashBcatDetector.Difference
{
    public class DifferenceTypeExtensions
    {
        public static DifferenceType[] GetAllDifferenceTypes()
        {
            return (DifferenceType[])Enum.GetValues(typeof(DifferenceType));
        }

    }
}