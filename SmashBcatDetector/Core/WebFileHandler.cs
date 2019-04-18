using System.Collections.Generic;
using System.IO;
using BcatBotFramework.Core.Config;
using Newtonsoft.Json;
using Renci.SshNet;
using SmashBcatDetector.Core.Config;
using SmashBcatDetector.Json;

namespace SmashBcatDetector.Core
{
    public static class WebFileHandler
    {
        public static object Lock = new object();

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

        public static void Connect()
        {
            // Get the WebConfig
            WebConfig webConfig = ((SsbuBotConfiguration)Configuration.LoadedConfiguration).WebConfig;

            // Check if the configuration specifies a remote server
            if (webConfig.RemoteServer != null)
            {
                // Create the SftpClient
                SftpClient = new SftpClient(webConfig.RemoteServer.Address, webConfig.RemoteServer.Username, new PrivateKeyFile(webConfig.RemoteServer.PrivateKey));
                SftpClient.Connect();
            }
        }

        public static void Disconnect()
        {
            if (SftpClient != null)
            {
                SftpClient.Disconnect();
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

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public static T FromJson<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, DeserializerSettings);
        }

    }
}