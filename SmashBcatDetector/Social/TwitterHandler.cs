using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SmashBcatDetector.Json.Config;
using Tweetinvi;
using Tweetinvi.Credentials.Models;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace SmashBcatDetector.Social
{
    public class TwitterHandler
    {
        public static void Initialize()
        {
            // Get the current configuration
            TwitterConfig twitterConfig = Configuration.LoadedConfiguration.TwitterConfig;
            
            // Declare a variable to hold the target credentials
            ITwitterCredentials userCredentials;

            // Check for default values on the token
            if (twitterConfig.Token == "cafebabe")
            {
                // Set up TwitterCredentials
                ITwitterCredentials twitterCredentials = new TwitterCredentials(twitterConfig.ConsumerKey, twitterConfig.ConsumerSecret);

                // Get an AuthenticationContext
                IAuthenticationContext authContext = AuthFlow.InitAuthentication(twitterCredentials);

                // Print out the URL
                Console.WriteLine("** An autentication token will be generated.");
                Console.WriteLine("** Please visit this URL: " + authContext.AuthorizationURL);
                Console.Write("\nPlease enter the PIN > ");

                // Wait for the PIN to be entered
                string pin = Console.ReadLine();

                // Get the credentials back from Twitter
                userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pin, authContext);

                // Get the token and token secret
                twitterConfig.Token = userCredentials.AccessToken;
                twitterConfig.TokenSecret = userCredentials.AccessTokenSecret;

                // Save the configuration
                Configuration.LoadedConfiguration.Write();
            }
            else
            {
                // Load the credentials from the configuration
                userCredentials = new TwitterCredentials(twitterConfig.ConsumerKey, twitterConfig.ConsumerSecret,
                    twitterConfig.Token, twitterConfig.TokenSecret);
            }

            // Set the user credentials
            Auth.SetCredentials(userCredentials);

            // Force the library to throw exceptions
            ExceptionHandler.SwallowWebExceptions = false;
        }

        public static void Dispose()
        {
            // TODO
        }

        public static void Tweet(string header, string text, string url, byte[] image = null)
        {
            // Check if Twitter is currently activated
            if (!Configuration.LoadedConfiguration.TwitterConfig.IsActivated)
            {
                // Do nothing
                return;
            }
            
#if DEBUG
            if (!Configuration.LoadedConfiguration.IsProduction)
            {
                // Generate four random bytes
                var bytes = new byte[4];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);
                }

                // Twitter doesn't accept duplicate statuses, so append 4 random bytes to make it unique
                header += " " + BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
#endif

            // Construct the dummy tweet
            string dummyTweet = header + "\n\n" + "\n\n" + url;

            // Declare truncated indicator
            string truncatedIndicator = " [...]";

            // Check if content needs to be truncated
            int lengthWithoutContent = ExecuteCharacterCounter(dummyTweet);
            int length = lengthWithoutContent + ExecuteCharacterCounter(text);
            if (length > 280)
            {
                int indicatorLength = ExecuteCharacterCounter(truncatedIndicator);

                // Truncate the content until it fits
                while (length > (280 - indicatorLength))
                {
                    text = text.Substring(0, text.Length - 1);
                    length = lengthWithoutContent + ExecuteCharacterCounter(text) + indicatorLength;
                }

                // Add the truncation indicator
                text += truncatedIndicator;
            }

            // Construct the final tweet
            string finalTweet = header + "\n\n" + text + "\n\n" + url;

            // Upload the media if needed
            IMedia media = null;
            if (image != null)
            {
                media = Upload.UploadBinary(image);
            }

            // Publish the tweet
            ITweet publishedTweet = Tweetinvi.Tweet.PublishTweet(finalTweet, new PublishTweetOptionalParameters()
            {
                Medias = (image == null) ? null : new List<IMedia>()
                {
                    media
                }
            });

            // Get the last exception and throw it (because the library is stupid and the option to automatically do this
            // doesn't work!!)
            Exception e = (Exception)ExceptionHandler.GetLastException();
            if (e != null)
            {
                throw e;
            }
        }

        private static int ExecuteCharacterCounter(string targetString)
        {
            // Create the Process
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            // Get the character counter binary
            string characterCounterBinary = Configuration.LoadedConfiguration.TwitterConfig.CharacterCounterBinary;

            // Set OS-dependent options
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                process.StartInfo.FileName = "/usr/bin/node";
                process.StartInfo.Arguments = characterCounterBinary + " \"" + targetString + "\"";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                process.StartInfo.FileName = characterCounterBinary;
                process.StartInfo.Arguments = "\"" + targetString + "\"";
            }
            else
            {
                throw new Exception("Unsupported OS");
            }

            // Start the process and wait
            process.Start();
            process.WaitForExit();

            // Return stdout
            return int.Parse(process.StandardOutput.ReadToEnd());
        }

    }
}