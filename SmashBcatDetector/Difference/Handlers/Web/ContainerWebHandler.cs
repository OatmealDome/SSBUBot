using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using SmashBcatDetector.Json;
using BcatBotFramework.Core.Config;
using SmashBcatDetector.S3;
using S3;
using Nintendo.SmashUltimate.Bcat;
using SmashBcatDetector.Core.Config;
using BcatBotFramework.Difference;
using ImageMagick;
using Renci.SshNet;

namespace SmashBcatDetector.Difference.Handlers.Web
{
    public class ContainerWebHandler
    {
        private static SftpClient SftpClient = null;

        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new ShouldSerializeContractResolver(),
            Converters = new List<JsonConverter>
            {
                new LanguageMappingDictionaryConverter(),
                new StringZeroByteTrimmerConverter()
            },
#if DEBUG
            Formatting = Newtonsoft.Json.Formatting.Indented
#endif
        };

        private static JsonSerializerSettings DeserializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new ShouldSerializeContractResolver(),
            Converters = new List<JsonConverter>
            {
                new LanguageMappingDictionaryConverter(),
            },
#if DEBUG
            Formatting = Newtonsoft.Json.Formatting.Indented
#endif
        };

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
            byte[] json = Encoding.UTF8.GetBytes(ToJson(container));

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

            // Connect to the remote server if needed
            ConnectRemoteServer();

            // Convert the Container to a StrippedContainer
            StrippedContainer strippedContainer = StrippedContainer.ConvertToStrippedContainer(container);

            // Declare a variable to hold the container list
            List<StrippedContainer> containerList;

            // Format the container list path
            string indexPath = string.Format(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerListPath, FileTypeExtensions.GetNamePrefixFromType(fileType));

            // Check if the file exists
            if (FileExists(indexPath))
            {
                // Deserialize the List
                containerList = FromJson<List<StrippedContainer>>(ReadFile(indexPath));
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
            WriteFile(indexPath, ToJson(containerList));

            // Declare a variable to hold the ContainerIndex
            ContainerIndex containerIndex;
            
            // Check if the ContainerIndex exists
            if (FileExists(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerIndexPath))
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
                containerIndex = FromJson<ContainerIndex>(ReadFile(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerIndexPath));
            }

            // Get the correct property
            PropertyInfo propertyInfo = containerIndex.GetType().GetProperty(container.GetType().Name);

            // Set the value
            propertyInfo.SetValue(containerIndex, strippedContainer);

            // Write out the ContainerIndex
            WriteFile(((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig.ContainerIndexPath, ToJson(containerIndex));
        
            // Disconnect from the remote server
            DisconnectRemoteServer();
        }

        [SsbuBotDifferenceHandler(FileType.Event, DifferenceType.Changed, 1)]
        [SsbuBotDifferenceHandler(FileType.LineNews, DifferenceType.Changed, 1)]
        [SsbuBotDifferenceHandler(FileType.PopUpNews, DifferenceType.Changed, 1)]
        [SsbuBotDifferenceHandler(FileType.Present, DifferenceType.Changed, 1)]
        public static void HandleChangedContainer(Container previousContainer, Container newContainer)
        {
            HandleContainer(newContainer);
        }

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        private static T FromJson<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, DeserializerSettings);
        }

        private static void ConnectRemoteServer()
        {
            // Get the WebConfig
            WebConfig webConfig = ((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig;

            // Check if the configuration specifies a remote server
            if (webConfig.RemoteServer == null)
            {
                // Create the SftpClient
                SftpClient = new SftpClient(webConfig.RemoteServer.Address, webConfig.RemoteServer.Username, new PrivateKeyFile(webConfig.RemoteServer.PrivateKey));
            }
        }

        private static void DisconnectRemoteServer()
        {
            if (SftpClient != null)
            {
                SftpClient.Dispose();
            }
        }

        private static bool FileExists(string path)
        {
            // Check if a remote connection isn't open
            if (SftpClient == null)
            {
                // Read from the local path
                return File.Exists(path);
            }
            else
            {
                // Read from the server
                return SftpClient.Exists(path);
            }
        }

        private static string ReadFile(string path)
        {
            // Check if a remote connection isn't open
            if (SftpClient == null)
            {
                // Read from the local path
                return File.ReadAllText(path);
            }
            else
            {
                // Read from the server
                return SftpClient.ReadAllText(path);
            }
        }

        private static void WriteFile(string path, string text)
        {
            // Check if a remote connection isn't open
            if (SftpClient == null)
            {
                // Write the file to the local path
                File.WriteAllText(path, text);
            }
            else
            {
                // Write to the server
                SftpClient.WriteAllText(path, text);
            }
        }

    }
}