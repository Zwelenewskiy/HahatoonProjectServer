using System;
using System.Data;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Globalization;

namespace HahatoonProjectServer
{
    public class Server
    {
        private HttpListener listener;

        public Server() { }
        public void NewUser(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;
            DateTime curDate = DateTime.Now;            
            Structs.Authentication enter;
            
            var request = context.Request;
            var response = context.Response;

            using (StreamReader input = new StreamReader( request.InputStream, Encoding.UTF8))
            {
                string Command = null, Jstr = null;
                Functions.Parser(input.ReadToEnd(), ref Command, ref Jstr, Structs.SeparatorChar);
                
                var connect = Functions.Connection(Structs.CONNECTION_FILE);

                connect.Open();
                if (connect.State != ConnectionState.Open)
                {
                    Functions.ShowError(Structs.Errors.ErrorCreatingConnection);
                    return;
                }

                switch (Convert.ToInt32(Command))
                {
                    /////////////////Авторизация/////////////////
                    case 0:
                        enter = JsonConvert.DeserializeObject<Structs.Authentication>(Jstr);

                        using (var Reader = Functions.Query(connect, $"select * from {Structs.BD_NAME}.user where login = @login && password = @password", true,
                            new Structs.Query("@login", null, enter.Login), new Structs.Query("@password", null, enter.Password)))
                        {
                            if (Reader.HasRows)
                            {
                                Functions.SendMessage(response, "1");

                                Console.WriteLine("[" + curDate + "] Подключен пользовaтель [" + enter.Login + "] [" + enter.Password + "]");
                                Console.WriteLine();
                            }
                            else
                                Functions.SendMessage(response, "0");
                                
                        }                        

                        break;

                    /////////////////Список INN-CompName/////////////////
                    case 1:
                        List<Structs.INN_Comp.Body_Element> tmp = new List<Structs.INN_Comp.Body_Element>();
                        var Login = JsonConvert.DeserializeObject<Structs.Authentication>(Jstr).Login;
                                                
                        using (var Reader = Functions.Query(connect, $"select inn, compName from {Structs.BD_NAME}.inn_comp where User_login = @login", true,
                            new Structs.Query("@login", null, Login)))
                        {
                            while (Reader.Read())
                            {
                                tmp.Add(new Structs.INN_Comp.Body_Element(Reader[0].ToString(), Reader[1].ToString()));
                            }

                            Functions.SendMessage(response, JsonConvert.SerializeObject(new Structs.INN_Comp(tmp)));
                        }

                        break;

                    case 2:
                        enter = JsonConvert.DeserializeObject<Structs.Authentication>(Jstr);

                        Console.WriteLine("[" + curDate + "] Пользовaтель [" + enter.Login + "] [" + enter.Password + "] отключен");
                        Console.WriteLine();

                        break;

                    case 3:
                        var report = JsonConvert.DeserializeObject<Structs.Report>(Jstr);

                        string query = $"call {Structs.BD_NAME}.AddNewReport('{report.inn}', {report.quarter}, {report.year}, '{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}', '" +
                            $"{report.param1[0]}', '{report.param2[0]}', '{report.param3[0].ToString("G", CultureInfo.InvariantCulture)}', '" +
                            $"{report.param1[1]}', '{report.param2[1]}', '{report.param3[1].ToString("G", CultureInfo.InvariantCulture)}', '" +
                            $"{report.param1[2]}', '{report.param2[2]}', '{report.param3[2].ToString("G", CultureInfo.InvariantCulture)}', '" +
                            $"{report.param1[3]}', '{report.param2[3]}', '{report.param3[3].ToString("G", CultureInfo.InvariantCulture)}', '" +
                            $"{report.param1[4]}', '{report.param2[4]}', '{report.param3[4].ToString("G", CultureInfo.InvariantCulture)}')";

                        Functions.Query(connect, query, false);

                        Functions.SendMessage(response, "1");

                        break;
                }

                connect.Close();
                connect.Dispose();
            }
        }

        public void Start(string host)
        {
            DateTime curDate = DateTime.Now;

            listener = new HttpListener();

            // установка адресов прослушки 
            listener.Prefixes.Add(host);
            listener.Start();

            Console.WriteLine(curDate + " Сервер запущен. Ожидание подключений...");
            Console.WriteLine();
            Console.WriteLine();

        }
        public void Stop()
        {
            // останавливаем прослушивание подключений 
            listener.Stop();
            Structs.server = null;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Сервер остановлен");
        }

        public void NewConnection()
        {
            // метод GetContext блокирует текущий поток, ожидая получение запроса 
            var context = listener.GetContext();

            Thread new_user = new Thread(new ParameterizedThreadStart(NewUser));
            new_user.Start(context);
        }

        
    }
}
