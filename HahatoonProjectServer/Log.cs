using System;
using System.IO;
using System.Text;

namespace HahatoonProjectServer
{
    static class Log
    {
        private static readonly string PATH = "Log.txt";

        public static void Write(string message)
        {
            using (var sw = new StreamWriter(PATH, true, Encoding.Default))
            {
                sw.WriteLine(message);
            }
        }
    }
}
