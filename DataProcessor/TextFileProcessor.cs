using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace DataProcessor
{
    public class TextFileProcessor
    {
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public TextFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            //using readAllText
            //putting file into memory
            string originalText = File.ReadAllText(InputFilePath);
            string processedText = originalText.ToUpperInvariant();

            //Write out the memory into a file
            File.WriteAllText(OutputFilePath, processedText);
        }
    }
}
