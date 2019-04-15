using System.Collections.Generic;
using System.Threading.Tasks;
using BcatBotFramework.Core;
using BcatBotFramework.Social.Discord;
using ImageMagick;
using Nintendo.SmashUltimate.Bcat;
using S3;

namespace SmashBcatDetector.Core.OneTime
{
    public class WebPConversionOneTimeTask : OneTimeTask
    {
        protected override async Task Run()
        {
            await DiscordBot.LoggingChannel.SendMessageAsync($"**[WebPConversionOneTimeTask]** Starting WebP image conversion");

            List<Container> allContainers = new List<Container>();

            // Add all Containers to the List
            allContainers.AddRange(ContainerCache.GetEvents());
            allContainers.AddRange(ContainerCache.GetPopUpNews());
            allContainers.AddRange(ContainerCache.GetPresents());

            // Loop over every Container
            foreach (Container container in allContainers)
            {
                // Get the FileType
                FileType fileType = FileTypeExtensions.GetTypeFromContainer(container);

                // Format the destination S3 path
                string s3Path = $"/smash/{FileTypeExtensions.GetNamePrefixFromType(fileType)}/{container.Id}";

                // Get the raw image
                byte[] jpgImage = (byte[])container.GetType().GetProperty("Image").GetValue(container);

                // Create a new MagickImage
                using (MagickImage image = new MagickImage(jpgImage))
                {
                    // Set the output format to WebP
                    image.Format = MagickFormat.WebP;

                    // Create the raw WebP
                    byte[] webpImage = image.ToByteArray();

                    await DiscordBot.LoggingChannel.SendMessageAsync($"**[WebPConversionOneTimeTask]** Uploading image for {fileType.ToString()} ID {container.Id}");
                    
                    // Upload to S3
                    S3Api.TransferFile(webpImage, s3Path, "image.webp", "image/webp");
                }
            }
        }

    }
}