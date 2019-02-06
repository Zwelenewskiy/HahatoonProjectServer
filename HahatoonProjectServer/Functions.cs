using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

                case Structs.Errors.ErrorStopServer:
                    Console.WriteLine("[" + DateTime.Now + "] Server is already stopped");
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

        public static void SendMessage(HttpListenerResponse response, string message)
        {
            response.ContentLength64 = Encoding.UTF8.GetBytes(message).Length;
            response.OutputStream.Write(Encoding.UTF8.GetBytes(message), 0, Encoding.UTF8.GetBytes(message).Length);
        }

        public static MySqlConnection Connection(string host, int port, string database, string username, string password)
        {
            return new MySqlConnection("server=" + host + ";database=" + database
               + ";port=" + port + ";user=" + username + ";password=" + password);

        }

        public static MySqlDataReader Query(MySqlConnection connect, string query, bool need, params Structs.Query[] parametrs)
        {
            using (var command = new MySqlCommand(query, connect))
            {
                if (need)
                {
                    for (int i = 0; i < parametrs.Length; i++)
                    {
                        if (parametrs[i].parametr != null)
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr));
                        else
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr1));
                    }

                    return command.ExecuteReader();

                }
                else
                {
                    for (int i = 0; i < parametrs.Length; i++)
                    {
                        if (parametrs[i].parametr != null)
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr));
                        else
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr1));
                    }

                    command.ExecuteReader();
                    return null;
                }
            }
        }

        /// <summary>
        /// Обрабатывает глобальную команду пользователя
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
                    
                    Structs.server = new Server();
                    Structs.server.Start(Structs.HOST);

                    break;

                case "stop":
                    if (Structs.server == null)
                    {
                        ShowError(Structs.Errors.ErrorStopServer);
                        return;
                    }

                    Structs.server.Stop();

                    break;
            }
        }
    }
}
