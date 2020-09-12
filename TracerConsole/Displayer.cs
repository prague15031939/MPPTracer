using System;
using System.IO;

namespace TracerConsole
{
    public interface IDisplayer
    {
        void Display(string result);
    }

    public class ConsoleDisplayer : IDisplayer
    {
        public void Display(string result)
        {
            Console.WriteLine(result);
        }
    }

    public class FileDisplayer : IDisplayer
    {
        private string FilePath { get; set; }

        public FileDisplayer(string path)
        {
            FilePath = path;
        }

        public void Display(string result)
        {
            using (StreamWriter fStream = new StreamWriter(FilePath))
            {
                fStream.Write(result);
            }
        }
    }
}
