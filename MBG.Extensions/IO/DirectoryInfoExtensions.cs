using System.Collections.Generic;
using System.IO;

namespace MBG.Extensions.IO
{
    public static class DirectoryInfoExtensions
    {
        // Many thanks to Keith Rull: http://www.keithrull.com/2008/02/04/AnotherRealWorldExampleOnWhenToUseExtensionMethods.aspx
        /// <summary>
        /// Get files using the specified pattern
        /// </summary>
        /// <param name="d">the DirectoryInfo object</param>
        /// <param name="listOfSearchPatterns"> the list of search patterns</param>
        /// <returns>returns a list of FileInfo objects</returns>
        public static IEnumerable<FileInfo> GetFiles(this DirectoryInfo directoryInfo, IEnumerable<string> searchPatterns)
        {
            List<FileInfo> matchingFiles = new List<FileInfo>();

            foreach (string pattern in searchPatterns)
            {
                matchingFiles.AddRange(directoryInfo.GetFiles(pattern));
            }

            return matchingFiles;
        }
        public static IEnumerable<FileInfo> GetFiles(this DirectoryInfo directoryInfo, params string[] searchPatterns)
        {
            List<FileInfo> matchingFiles = new List<FileInfo>();

            foreach (string searchPattern in searchPatterns)
            {
                matchingFiles.AddRange(directoryInfo.GetFiles(searchPattern));
            }

            return matchingFiles;
        }

        // Many thanks to Keith Rull: http://www.keithrull.com/2008/02/04/AnotherRealWorldExampleOnWhenToUseExtensionMethods.aspx
        /// <summary>
        /// Delete files using a pattern
        /// </summary>
        /// <param name="d">the directory info</param>
        /// <param name="listOfSearchPatterns">the list of search patterns</param>
        public static void DeleteFiles(this DirectoryInfo directoryInfo, IEnumerable<string> searchPatterns)
        {
            IEnumerable<FileInfo> matchingFiles = GetFiles(directoryInfo, searchPatterns);

            foreach (FileInfo matchingFile in matchingFiles)
            {
                if (File.Exists(matchingFile.FullName))
                {
                    File.Delete(matchingFile.FullName);
                }
            }
        }
        public static void DeleteFiles(this DirectoryInfo directoryInfo, params string[] searchPatterns)
        {
            foreach (FileInfo matchingFile in GetFiles(directoryInfo, searchPatterns))
            {
                if (File.Exists(matchingFile.FullName))
                {
                    File.Delete(matchingFile.FullName);
                }
            }
        }

        /// <summary>
        /// Creates the directory, if it does not already exist
        /// </summary>
        /// <param name="directoryInfo"></param>
        public static void Ensure(this DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }
}