using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HahatoonProjectServer
{
    class Program
    {
        readonly static string HOST = "http://localhost:8888/";

        public static void Parser(string Input, ref string Command, ref string Jstring, char Separator)
        {
            int i = 0;
            while (Input[i] != Separator)
            {
                Command += Input[i];
                i++;
            }

            for (int j = ++i; j < Input.Length; j++)
                Jstring += Input[j];
        }

        public static void Connection()
        {

        }
        
        public static void ReadCommand()
        {
            string command = Console.ReadLine();

            switch (command.ToLower())
            {
                case "stat":
                    break;

                case "clear":
                    Console.Clear();
                    break;
            }
        }

        static void Main(string[] args)
        {
            Task.Factory.StartNew(() => 
            {
                Server server = new Server();
                server.Start(HOST);

                while (true)
                {
                    server.NewConnection();
                }
            }, TaskCreationOptions.LongRunning);


            while (true)
            {
                ReadCommand();
            }
        }       
    }
}
