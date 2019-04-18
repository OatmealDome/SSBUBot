using System.IO;
using BcatBotFramework.Core.Config;
using Renci.SshNet;
using SmashBcatDetector.Core.Config;

namespace SmashBcatDetector.Core
{
    public static class WebFileHandler
    {
        public static object Lock = new object();

        private static SftpClient SftpClient = null;

        public static void Connect()
        {
            // Get the WebConfig
            WebConfig webConfig = ((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig;

            // Check if the configuration specifies a remote server
            if (webConfig.RemoteServer != null)
            {
                // Create the SftpClient
                SftpClient = new SftpClient(webConfig.RemoteServer.Address, webConfig.RemoteServer.Username, new PrivateKeyFile(webConfig.RemoteServer.PrivateKey));
            }
        }

        public static void Disconnect()
        {
            if (SftpClient != null)
            {
                SftpClient.Dispose();
                SftpClient = null;
            }
        }

        public static bool Exists(string path)
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

        public static string ReadAllText(string path)
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

        public static void WriteAllText(string path, string text)
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