using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    public class BinaryFileProcessor
    {
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public BinaryFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            byte[] data = File.ReadAllBytes(InputFilePath);
            byte largestByte = data.Max();


            byte[] newData = new byte[data.Length + 1];
            Array.Copy(data, newData, data.Length);
            newData[newData.Length - 1] = largestByte;

            File.WriteAllBytes(OutputFilePath, newData);
        }
    }
}
