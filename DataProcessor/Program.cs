using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataProcessor
{
    class Program
    {
        public static MemoryCache FilesToProcess = MemoryCache.Default;

        static void Main(string[] args)
        {
            Console.WriteLine("EXECUTING COMMAND LINE ARGUMENTS args[0] ");

            /*automated way using FileSystemWatcher*/
            var directoryToWatch = args[0];
            if (!Directory.Exists(directoryToWatch))
            {
                Console.WriteLine($"ERROR: {directoryToWatch} does not exist");
            }
            else
            {
                Console.WriteLine($"WATCHING: {directoryToWatch} for changes");
                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                {
                    //does not watch for chanages in the sub dirs
                    inputFileWatcher.IncludeSubdirectories = false;
                    inputFileWatcher.InternalBufferSize = (1024 * 32); //32 KB 
                    inputFileWatcher.Filter = "*.*"; //monitors all files default is the same
                    inputFileWatcher.NotifyFilter = NotifyFilters.Size;
                    inputFileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;// | NotifyFilters.Size;

                    //bydefault eventRaising Property is set False
                    inputFileWatcher.EnableRaisingEvents = true;

                    //eventhandlers
                    inputFileWatcher.Created += InputFileWatcher_Created;
                    inputFileWatcher.Changed += InputFileWatcher_Changed;
                    inputFileWatcher.Deleted += InputFileWatcher_Deleted;
                    inputFileWatcher.Renamed += InputFileWatcher_Renamed;
                    inputFileWatcher.Error += InputFileWatcher_Error;

                    Console.WriteLine("Press any key to quit");
                    Console.ReadKey();
                }
            }






        }

        private static void InputFileWatcher_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"ERROR: {e.GetException()}: {DateTime.Now}");
        }

        private static void InputFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File Renamed: {e.OldName} to {e.Name} - type {e.ChangeType} : {DateTime.Now}");
        }

        private static void InputFileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File Deleted: {e.Name} - type {e.ChangeType}: {DateTime.Now}");
        }

        private static void InputFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File Changed: {e.Name} - type {e.ChangeType}: {DateTime.Now}");
            AddToCache(e.FullPath);
        }

        private static void InputFileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File Created: {e.Name} - type {e.ChangeType}: {DateTime.Now}");
            AddToCache(e.FullPath);
        }

        private static void AddToCache(string fullPath)
        {
            //place item into the cache
            var item = new CacheItem(fullPath, fullPath); //key, object

            //specify how long the item will stay in the cache
            var policy = new CacheItemPolicy
            {
                //Removedcallback : what happens when item is removed the cache, pointer to a function
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2)  ///have to specific more than 1 second, else it wont be updated
            };

            FilesToProcess.Add(item, policy);

        }


        /// <summary>
        /// Process the File using Caching 
        /// </summary>
        /// <param name="arguments"></param>
        private static void ProcessFile(CacheEntryRemovedArguments arguments)
        {
            Console.WriteLine($"* Cache item removed: {arguments.CacheItem.Key} due to {arguments.RemovedReason}");

            if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(arguments.CacheItem.Key);
                fileProcessor.Process();
            }
            else
            {
                Console.WriteLine($"WARNING: {arguments.CacheItem.Key} was removed unexpectedly");
            }
        }


        private static void ProcessSingleFile(string filepath)
        {
            var fileProcessor = new FileProcessor(filepath);
            fileProcessor.Process();
        }

        private static void ProcessDirectory(string dirpath, string fileType)
        {
            //var allFiles = Directory.GetFiles(dirpath);

            switch (fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(dirpath, "*.txt");
                    foreach (var textFilePath in textFiles)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    Console.WriteLine($"ERROR: {fileType} is NOT supported");
                    return;
            }


        }
    }
}
