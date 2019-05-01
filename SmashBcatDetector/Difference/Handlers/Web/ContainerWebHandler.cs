using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using BcatBotFramework.Core.Config;
using SmashBcatDetector.S3;
using S3;
using Nintendo.SmashUltimate.Bcat;
using SmashBcatDetector.Core.Config;
using BcatBotFramework.Difference;
using ImageMagick;
using Renci.SshNet;
using SmashBcatDetector.Core;
using BcatBotFramework.Core;

namespace SmashBcatDetector.Difference.Handlers.Web
{
    public class ContainerWebHandler
    {
        [SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Added, 1)]
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Added, 1)]
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Added, 1)]
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Added, 1)]
        public static void HandleContainer(Container container)
        {
            // Get the FileType
            FileType fileType = FileTypeExtensions.GetTypeFromContainer(container);

            // Format the destination S3 path
            string s3Path = $"/smash/{FileTypeExtensions.GetNamePrefixFromType(fileType)}/{container.Id}";

            // Convert the Container into a JSON string
            byte[] json = Encoding.UTF8.GetBytes(WebFileHandler.ToJson(container));

            // Write the data to S3
            S3Api.TransferFile(json, s3Path, "data.json");

            // Check if this has an image
            if (fileType == FileType.Event || fileType == FileType.PopUpNews || fileType == FileType.Present)
            {
                // Get the image
                byte[] image = (byte[])container.GetType().GetProperty("Image").GetValue(container);

                // Write the image to S3
                S3Api.TransferFile(image, s3Path, "image.jpg", "image/jpeg");

                // Create a new MagickImage
                using (MagickImage magickImage = new MagickImage(image))
                {
                    // Set the output format to WebP
                    magickImage.Format = MagickFormat.WebP;

                    // Create the raw WebP
                    byte[] webpImage = magickImage.ToByteArray();
                    
                    // Upload to S3
                    S3Api.TransferFile(webpImage, s3Path, "image.webp", "image/webp");
                }
            }

            lock (WebFileHandler.Lock)
            {
                // Connect to the remote server if needed
                WebFileHandler.Connect(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig);

                // Convert the Container to a StrippedContainer
                StrippedContainer strippedContainer = StrippedContainer.ConvertToStrippedContainer(container);

                // Declare a variable to hold the container list
                List<StrippedContainer> containerList;

                // Format the container list path
                string indexPath = string.Format(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerListPath, FileTypeExtensions.GetNamePrefixFromType(fileType));

                // Check if the file exists
                if (WebFileHandler.Exists(indexPath))
                {
                    // Deserialize the List
                    containerList = WebFileHandler.ReadAllText<List<StrippedContainer>>(indexPath);
                }
                else
                {
                    // Create a new List
                    containerList = new List<StrippedContainer>();
                }

                // Check if the Container already exists in the list
                int index = containerList.FindIndex(x => x.Id == container.Id);

                // Check the index
                if (index == -1)
                {
                    // Add the StrippedContainer to the List
                    containerList.Add(strippedContainer);
                }
                else
                {
                    // Replace the item at the index
                    containerList[index] = strippedContainer;
                }
                
                // Serialize and write the container list
                WebFileHandler.WriteAllText(indexPath, containerList);

                // Declare a variable to hold the ContainerIndex
                ContainerIndex containerIndex;
                
                // Check if the ContainerIndex exists
                if (!WebFileHandler.Exists(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerIndexPath))
                {
                    // Create a dummy StrippedContainer
                    StrippedContainer dummyStrippedContainer = new StrippedContainer();
                    dummyStrippedContainer.Id = "-1";
                    dummyStrippedContainer.Text = new Dictionary<Nintendo.Bcat.Language, string>();

                    // Create a dummy ContainerIndex
                    containerIndex = new ContainerIndex();
                    containerIndex.Event = dummyStrippedContainer;
                    containerIndex.LineNews = dummyStrippedContainer;
                    containerIndex.PopUpNews = dummyStrippedContainer;
                    containerIndex.Present = dummyStrippedContainer;
                }
                else
                {
                    // Read the file
                    containerIndex = WebFileHandler.ReadAllText<ContainerIndex>(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerIndexPath);
                }

                // Get the correct property
                PropertyInfo propertyInfo = containerIndex.GetType().GetProperty(container.GetType().Name);

                // Set the value
                propertyInfo.SetValue(containerIndex, strippedContainer);

                // Write out the ContainerIndex
                WebFileHandler.WriteAllText(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerIndexPath, containerIndex);
            
                // Disconnect from the remote server
                WebFileHandler.Disconnect();
            }
        }

        [SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Changed, 1)]
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Changed, 1)]
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Changed, 1)]
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Changed, 1)]
        public static void HandleChangedContainer(Container previousContainer, Container newContainer)
        {
            HandleContainer(newContainer);
        }

    }
}