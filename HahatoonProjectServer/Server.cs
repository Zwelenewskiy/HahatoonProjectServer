﻿using System;
using System.Data;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

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
            Structs.Authentication Enter;
            
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
                        Enter = JsonConvert.DeserializeObject<Structs.Authentication>(Jstr);
                        
                        using (var Reader = Query(connect, "select Type from project.users where login = @login && password = @password", true,
                            new Structs.Query("@login", null, Enter.Login), new Structs.Query("@password", null, Enter.Password)))
                        {
                            if (Reader.HasRows)
                            {

                                /*while (Reader.Read())
                                {
                                    Console.WriteLine("Type = " + Reader[0]);
                                }*/

                                SendMessage(response, "1");

                                Console.WriteLine("[" + curDate + "] Подключен пользовaтель " + Enter.Login + " " + Enter.Password);
                                Console.WriteLine();
                            }
                            else
                                SendMessage(response, "0");
                                
                        }                        

                        break;

                    /////////////////Список INN-CompName/////////////////
                    case 1:
                        List<Structs.INN_Comp.Body_Element> tmp = new List<Structs.INN_Comp.Body_Element>();
                        var Login = JsonConvert.DeserializeObject<Structs.Authentication>(Jstr).Login;

                        using (var Reader = Query(connect, "select inn, comp from project.inn_comp where login = @login", true,
                            new Structs.Query("@login", null, Login)))
                        {
                            while (Reader.Read())


                            {
                                tmp.Add(new Structs.INN_Comp.Body_Element(Reader[0].ToString(), Reader[1].ToString()));
                            }
                                                    
                            SendMessage(response, JsonConvert.SerializeObject(new Structs.INN_Comp(tmp)));
                        }

                        break;

                    case 2:
                        Enter = JsonConvert.DeserializeObject<Structs.Authentication>(Jstr);

                        Console.WriteLine(curDate + " Пользовaтель " + Enter.Login + " " + Enter.Password + " отключен");
                        Console.WriteLine();

                        break;

                    case 3:
                        var Report = JsonConvert.DeserializeObject<Structs.Report>(Jstr);

                        /*string query = $"insert into project.reports value('{Quarter}', '{dateNow.ToString("yyyy.MM.dd HH:mm:ss")}', '" +
                    $"{ValParams.param1[0]}', '{ValParams.param2[0]}', '" +
                    $"{ValParams.param3[0].ToString("G", CultureInfo.InvariantCulture)}', '" +
                    $"{ValParams.param1[1]}', '{ValParams.param2[1]}', '" +
                    $"{ValParams.param3[1].ToString("G", CultureInfo.InvariantCulture)}', '" +
                    $"{ValParams.param1[2]}', '{ValParams.param2[2]}', '" +
                    $"{ValParams.param3[2].ToString("G", CultureInfo.InvariantCulture)}', '" +
                    $"{ValParams.param1[3]}', '{ValParams.param2[3]}', '" +
                    $"{ValParams.param3[3].ToString("G", CultureInfo.InvariantCulture)}', '" +
                    $"{ValParams.param1[4]}', '{ValParams.param2[4]}', '" +
                    $"{ValParams.param3[4].ToString("G", CultureInfo.InvariantCulture)}')"; */

                        SendMessage(response, "1");

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
        public void SendMessage(HttpListenerResponse response, string message)
        {
            response.ContentLength64 = Encoding.UTF8.GetBytes(message).Length;
            response.OutputStream.Write(Encoding.UTF8.GetBytes(message), 0, Encoding.UTF8.GetBytes(message).Length);
        }

        public MySqlConnection Connection(string host, int port, string database, string username, string password)
        {
            return new MySqlConnection("server=" + host + ";database=" + database
               + ";port=" + port + ";user=" + username + ";password=" + password);

        }   
        
        public MySqlDataReader Query(MySqlConnection connect, string query, bool need, params Structs.Query[] parametrs)
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
    }
}
