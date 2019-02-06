using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        public static void ShowMessage(Structs.Message messageType)
        {
            Console.WriteLine();
            Console.WriteLine();

            switch (messageType)
            {
                case Structs.Message.ConnectionFileNotExists:                    
                    Console.WriteLine("[" + DateTime.Now + "] File with connection settings not found");   
                    break;

                case Structs.Message.ErrorCreatingConnection:
                    Console.WriteLine("[" + DateTime.Now + "] Error creating database connection");
                    break;

                case Structs.Message.ErrorStartServer:
                    Console.WriteLine("[" + DateTime.Now + "] Server is already running");
                    break;

                case Structs.Message.ErrorStopServer:
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
                ShowMessage(Structs.Message.ConnectionFileNotExists);

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
                        ShowMessage(Structs.Message.ErrorStartServer);
                        return;
                    }
                    
                    Structs.server = new Server();
                    Structs.server.Start(Structs.HOST);

                    break;

                case "stop":
                    if (Structs.server == null)
                    {
                        ShowMessage(Structs.Message.ErrorStopServer);
                        return;
                    }

                    Structs.server.Stop();

                    break;
            }
        }

        /// <summary>
        /// Получает текущую дату через интернет
        /// </summary>
        //public static DateTime GetNetworkTime()
        public static string GetNetworkTime()
        {
            const string ntpServer = "time.windows.com";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                socket.Send(ntpData);
                socket.Receive(ntpData);

                socket.Close();
            }

            var intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
            var fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            var currentTime = networkDateTime.ToLocalTime();

            return currentTime.Year + "." + currentTime.Month + "." + currentTime.Day + " " + currentTime.Hour + ":" + currentTime.Minute + ":" + currentTime.Second;
        }
    }
}
