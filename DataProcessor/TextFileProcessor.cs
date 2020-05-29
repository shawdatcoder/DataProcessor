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
            ////using readAllText
            ////putting file into memory
            //string originalText = File.ReadAllText(InputFilePath);
            //string processedText = originalText.ToUpperInvariant();

            ////Write out the memory into a file
            //File.WriteAllText(OutputFilePath, processedText);

            //using readAllLines
            string[] lines = File.ReadAllLines(InputFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if(i%2 ==  0 )
                lines[i] = lines[i].ToUpperInvariant();
            }
            //this always places a newline at the end of processing
            File.WriteAllLines(OutputFilePath, lines);
        }
    }
}
