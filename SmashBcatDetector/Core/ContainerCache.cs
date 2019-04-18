using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nintendo.SmashUltimate.Bcat;

namespace SmashBcatDetector.Core
{
    public class ContainerCache
    {
        // Initialized flag
        private static bool Initialized = false;

        // Accessibility flag and lock
        private static object CacheLock = new object();

        // Paths
        private static string CachePath;

        // Files
        private static List<Event> Events;
        private static List<LineNews> LineNews;
        private static List<PopUpNews> PopUpNews;
        private static List<Present> Presents;

        public static void Initialize(string cachePath)
        {
            // Check initialization
            if (Initialized)
            {
                throw new Exception("Cannot initialize when already initialized");
            }

            // Set path
            CachePath = cachePath;

            // Create Dictionary and Lists
            Events = new List<Event>();
            LineNews = new List<LineNews>();
            PopUpNews = new List<PopUpNews>();
            Presents = new List<Present>();

            // Check if the path doesn't exist
            if (!Directory.Exists(CachePath))
            {
                // Create the Directory
                Directory.CreateDirectory(CachePath);

                return;
            }

            // Load every file in the cache path
            foreach (string path in Directory.GetFiles(CachePath))
            {
                // Get the FileType
                FileType fileType = FileTypeExtensions.GetTypeFromName(Path.GetFileName(path));

                // Open a FileStream
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    // Create the appropriate class and add to the correct List
                    switch (fileType)
                    {
                        case FileType.Event:
                            Events.Add(new Event(fileStream));
                            break;
                        case FileType.LineNews:
                            LineNews.Add(new LineNews(fileStream));
                            break;
                        case FileType.PopUpNews:
                            PopUpNews.Add(new PopUpNews(fileStream));
                            break;
                        case FileType.Present:
                            Presents.Add(new Present(fileStream));
                            break;
                    }
                }
            }

            // Sort the containers
            Events.Sort((x, y) => y.Id.CompareTo(x.Id));
            Presents.Sort((x, y) => y.Id.CompareTo(x.Id));
            PopUpNews.Sort((x, y) => y.Id.CompareTo(x.Id));
            Presents.Sort((x, y) => y.Id.CompareTo(x.Id));

            // Set initialized
            Initialized = true;
        }

        public static void Dispose()
        {
            if (!Initialized)
            {
                throw new Exception("Cannot dispose when not initialized");
            }

            // Clear lists
            Events = null;
            LineNews = null;
            PopUpNews = null;
            Presents = null;

            // Clear initialized flag
            Initialized = false;
        }

        public static void AddFile(Container container, string fileName, byte[] rawContainer)
        {
            lock (CacheLock)
            {
                // Add the Container to the correct List
                if (container is Event)
                {
                    Events.Add((Event)container);
                }
                else if (container is LineNews)
                {
                    LineNews.Add((LineNews)container);
                }
                else if (container is PopUpNews)
                {
                    PopUpNews.Add((PopUpNews)container);
                }
                else if (container is Present)
                {
                    Presents.Add((Present)container);
                }
                else
                {
                    throw new Exception("Unknown container type");
                }

                // Write out the file
                File.WriteAllBytes(Path.Combine(CachePath, fileName), rawContainer);
            }
        }

        public static Container OverwriteFile(Container container, string fileName, byte[] rawContainer)
        {
            lock (CacheLock)
            {
                // Declare a variable to hold the target index
                int idx;

                // Declare a variable to hold the previous Container
                Container previousContainer;
                
                // Add the Container to the correct List
                // TODO: there must be a better way
                if (container is Event)
                {
                    // Get the index
                    idx = Events.FindIndex(x => x.Id == container.Id);
                    
                    // Get the previous Container
                    previousContainer = Events[idx];

                    // Set the new Container
                    Events[idx] = (Event)container;
                }
                else if (container is LineNews)
                {
                    idx = LineNews.FindIndex(x => x.Id == container.Id);
                    previousContainer = LineNews[idx];
                    LineNews[idx] = (LineNews)container;
                }
                else if (container is PopUpNews)
                {
                    idx = PopUpNews.FindIndex(x => x.Id == container.Id);
                    previousContainer = PopUpNews[idx];
                    PopUpNews[idx] = (PopUpNews)container;
                }
                else if (container is Present)
                {
                    idx = Presents.FindIndex(x => x.Id == container.Id);
                    previousContainer = Presents[idx];
                    Presents[idx] = (Present)container;
                }
                else
                {
                    throw new Exception("Unknown container type");
                }

                return previousContainer;
            }
        }

        public static IList<Event> GetEvents()
        {
            lock (CacheLock)
            {
                return Events.AsReadOnly();                
            }
        }

        public static IList<LineNews> GetLineNews()
        {
            lock (CacheLock)
            {
                return LineNews.AsReadOnly();
            }
        }

        public static IList<PopUpNews> GetPopUpNews()
        {
            lock (CacheLock)
            {
                return PopUpNews.AsReadOnly();
            }
        }
        
        public static IList<Present> GetPresents()
        {
            lock (CacheLock)
            {
                return Presents.AsReadOnly();
            }
        }

        public static Event GetEventWithId(string id)
        {
            lock (CacheLock)
            {
                return Events.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static LineNews GetLineNewsWithId(string id)
        {
            lock (CacheLock)
            {
                return LineNews.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static PopUpNews GetPopUpNewsWithId(string id)
        {
            lock (CacheLock)
            {
                return PopUpNews.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public static Present GetPresentWithId(string id)
        {
            lock (CacheLock)
            {
                return Presents.Where(x => x.Id == id).FirstOrDefault();
            }
        }

    }
}