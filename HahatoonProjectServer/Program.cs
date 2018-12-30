using System;

namespace HahatoonProjectServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start("http://localhost:8888/");

            while (true)
            {
                server.NewConnection();
            }
        }

        public static void Parser(string Input, ref string Command, ref string Jstring, char Separator)
        {
            int i = 0;
            while(Input[i] != Separator)
            {
                Command += Input[i];
                i++;
            }                

            for (int j = ++i; j < Input.Length; j++)
                Jstring += Input[j];
        }
    }
}
