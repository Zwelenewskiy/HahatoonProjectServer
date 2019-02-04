using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Выводит сообщение об ошибке
        /// </summary>
        public static void ShowError(Structs.Errors error)
        {
            Console.WriteLine();
            Console.WriteLine();

            switch (error)
            {
                case Structs.Errors.ConnectionFileNotExists:                    
                    Console.WriteLine("[" + DateTime.Now + "] File with connection settings not found");                 

                    break;

                case Structs.Errors.ErrorCreatingConnection:
                    Console.WriteLine("[" + DateTime.Now + "] Error creating database connection");

                    break;

                case Structs.Errors.ErrorStartServer:
                    Console.WriteLine("[" + DateTime.Now + "] Server is already running");

                    break;
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Создает подключение к базе данных
        /// </summary>
        public static MySqlConnection Connection(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(Structs.CONNECTION_FILE, Encoding.Default))
                {
                    return new MySqlConnection("server=" + sr.ReadLine() + ";database=" + sr.ReadLine()
                        + ";port=" + Convert.ToInt32(sr.ReadLine()) + ";user=" + sr.ReadLine() + ";password=" + sr.ReadLine());
                }
            }
            else
            {
                ShowError(Structs.Errors.ConnectionFileNotExists);

                return null;
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

                case "start":
                    if(Structs.server != null)
                    {
                        ShowError(Structs.Errors.ErrorStartServer);
                        return;
                    }

                    Structs.server.Start(Structs.HOST);

                    break;
            }
        }
    }
}
