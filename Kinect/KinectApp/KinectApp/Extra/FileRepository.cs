using System;
using System.Configuration;
using System.IO;

namespace KinectApp.Extra
{
    public static class FileRepository
    {
        private static readonly string SaveLocation = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["SavedProgramsPath"]);

        /// <summary>
        /// Saves the program to the location specified in the config file. The filename is the date.
        /// Can throw a lot of different exceptions!
        /// </summary>
        /// <param name="content">The content to be written</param>
        public static void SaveProgram(string content)
        {
            SaveTextFile(Path.Combine(SaveLocation, DateTime.Now.ToString("ddMMyy-HHmmss") + ".json"), content);
        }

        /// <summary>
        /// Saves a text file on the specifide location.
        /// Can throw a lot of different exceptions!
        /// </summary>
        /// <param name="path">The path to the file. Filename included!</param>
        /// <param name="content">The file content</param>
        public static void SaveTextFile(string path, string content)
        {
            // Check save location
            if (!Directory.Exists(SaveLocation))
            {
                Directory.CreateDirectory(SaveLocation);
            }

            // Write the program
            var file = File.CreateText(path);
            file.WriteLine(content);
            file.Flush();
        }
    }
}
