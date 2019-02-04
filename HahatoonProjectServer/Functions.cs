using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahatoonProjectServer
{
    static class Functions
    {
        /// <summary>
        /// Парсит входящий запрос от клиента
        /// </summary>
        public static void Parser(string Input, ref string Command, ref string Jstring, char Separator)
        {
            /*
            <CODE_CHAR><SEPARATOR_CHAR><JSON>            
             */

            int i = 0;
            while (Input[i] != Separator)
            {
                Command += Input[i];
                i++;
            }

            for (int j = ++i; j < Input.Length; j++)
                Jstring += Input[j];
        }

        public static void ShowError(Structs.Errors error)
        {
            switch (error)
            {
                case Structs.Errors.FileNotExists:
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("[" + DateTime.Now + "] File with connection settings");
                    Console.WriteLine();
                    Console.WriteLine();

                    break;
            }
        }            

        /// <summary>
        /// Обрабатывает команду пользователя
        /// </summary>
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
    }
}
