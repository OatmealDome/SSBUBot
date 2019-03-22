using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmashUltimate.Bcat;

namespace SmashBcatDetector.Difference
{
    public class HandlerMapper
    {
        private static bool Initialized = false;

        // Dictionary
        private static Dictionary<FileType, Dictionary<DifferenceType, SortedList<int, MethodInfo>>> typeCatalog;

        public static void Initialize()
        {
            // Check initialization
            if (Initialized)
            {
                throw new Exception("Cannot initialize when already initialized");
            }

            // Initialize the outer Dictionary
            typeCatalog = new Dictionary<FileType, Dictionary<DifferenceType, SortedList<int, MethodInfo>>>();

            // Perform initial population
            foreach (FileType fileType in FileTypeExtensions.GetAllFileTypes())
            {
                // Add a blank Dictionary
                typeCatalog.Add(fileType, new Dictionary<DifferenceType, SortedList<int, MethodInfo>>());

                // Loop over each DifferenceType
                foreach (DifferenceType differenceType in DifferenceTypeExtensions.GetAllDifferenceTypes())
                {
                    // Create a new List
                    typeCatalog[fileType][differenceType] = new SortedList<int, MethodInfo>();
                }
            }

            // Get ourselves
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get all methods with the DifferenceHandler assembly
            IEnumerable<MethodInfo> methodInfos = assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(method => method.GetCustomAttributes(typeof(DifferenceHandlerAttribute), false).Length > 0);

            // Loop over every method
            foreach (MethodInfo methodInfo in methodInfos)
            {
                // Get the attributes for this method
                IEnumerable<Attribute> attributes = methodInfo.GetCustomAttributes(typeof(DifferenceHandlerAttribute));

                foreach (Attribute attribute in attributes)
                {
                    // Cast the attribute to DifferenceHandlerAttribute
                    DifferenceHandlerAttribute diffAttribute = (DifferenceHandlerAttribute)attribute;

                    // Add this to the Dictionary
                    typeCatalog[diffAttribute.FileType][diffAttribute.DifferenceType].Add(diffAttribute.Priority, methodInfo);
                }
                
            }

            // Set initialized flag
            Initialized = true;
        }

        public static void Dispose()
        {
            // Check initialization
            if (!Initialized)
            {
                throw new Exception("Cannot dispose when not initialized");
            }

            // Null out the Dictionary
            typeCatalog = null;
        }

        public static SortedList<int, MethodInfo> GetHandlers(FileType fileType, DifferenceType differenceType)
        {
            return typeCatalog[fileType][differenceType];
        }

    }
}