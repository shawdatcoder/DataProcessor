using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    internal class FileProcessor
    {
        private static readonly string BackupDirectoryName = "backup";
        private static readonly string InProgressDirectoryName = "processing";
        private static readonly string CompletedDirectoryName = "completed";
        public string InputFilePath { get; set; }

        public FileProcessor(string filepath)
        {
            InputFilePath = filepath;
        }

        public void Process()
        {


            Console.WriteLine($"Begining Single File Process of: {InputFilePath}" + "\r\n");

            //get root dir
            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            Console.WriteLine($"Root Directory Path is : {rootDirectoryPath}" + "\r\n");
            //check to see if file exists       
            if (!File.Exists(InputFilePath))
            {
                Console.WriteLine($"ERROR: {InputFilePath} Does Not Exist." );
                return;
            }
            //check if backup dir exists
            string inputFileDirectoryPath = Path.GetDirectoryName(InputFilePath);
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

            //check if backup dir exist if it doesnt exist create the dir
            if (!Directory.Exists(backupDirectoryPath))
            {
                Console.WriteLine($"Creating backup dir: {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            }

            //copy the file in case processing errors
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            Console.WriteLine($"Copying {InputFilePath} to {backupFilePath}");

            File.Copy(InputFilePath, backupFilePath, true);


            //move to processing folder
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            string inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            if (File.Exists(inProgressFilePath))
            {
               WriteLine($"ERROR: File with name {inProgressFilePath} already being processed!");
            }

            WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);



            //Determine the extension of the File
            string extension = Path.GetExtension(InputFilePath);


            string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
            Directory.CreateDirectory(completedDirectoryPath);


            var completedFileName = $"{ Path.GetFileNameWithoutExtension(InputFilePath)}_{Guid.NewGuid()}{extension}";
            //change the extension
            //completedFileName = Path.ChangeExtension(completedFileName, ".complete");
            var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);




            //swithcing based on ext
            switch (extension)
            {
                case ".txt":
                    var textFileProcessor = new TextFileProcessor(inProgressFilePath, completedFilePath);
                    textFileProcessor.Process();
                    break;

                default:
                    WriteLine($"{extension} is not supported");
                    break;

            }



            //File.Move(inProgressFilePath, completedFilePath);
            //Console.WriteLine($"Processing Completed of {completedFilePath}");


            WriteLine($"Completed the processing of {inProgressFilePath}");
            WriteLine($"Deleting {inProgressFilePath}");
            File.Delete(inProgressFilePath);

        }

       
    }
}