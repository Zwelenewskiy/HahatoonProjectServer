using System;
using System.Data;
using System.Drawing.Imaging;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace HahatoonProjectServer
{
    struct Report
    {
        string INN;
        public int FM1, FM2, GF1, GF2, CKR1, CKR2, CPP1, CPP2, CE1, CE2;
        public double FM3, GF3, CKR3, CPP3, CE3;

        public Report(string inn, int fm1, int fm2, double fm3, int gf1, int gf2, double gf3, int ckr1, int ckr2,
            double ckr3, int cpp1, int cpp2, double cpp3, int ce1, int ce2, double ce3)
        {
            INN = inn;

            FM1 = fm1;
            FM2 = fm2;
            FM3 = fm3;

            GF1 = gf1;
            GF2 = gf2;
            GF3 = gf3;

            CKR1 = ckr1;
            CKR2 = ckr2;
            CKR3 = ckr3;

            CPP1 = cpp1;
            CPP2 = cpp2;
            CPP2 = cpp2;
            CPP3 = cpp3;

            CE1 = ce1;
            CE2 = ce2;
            CE3 = ce3;
        }
    }

    struct Authentication
    {
        public string Login, Password;

        public Authentication(string log, string pass)
        {
            Login = log;
            Password = pass;
        }
    }

    struct INN_Comp
    {
        public struct Body_Element
        {
            public string INN, Comp_name;

            public Body_Element(string inn, string name)
            {
                INN = inn;
                Comp_name = name;
            }
        } 

        public List<Body_Element> body;       

        public INN_Comp(List<Body_Element> mas)
        {
            body = mas;
        }
    }

    struct Query
    {
        public double parametr;
        public string parametr1, name;

        public Query(string n, double p, string p1)
        {
            name = n;
            parametr = p;
            parametr1 = p1;
        }
    }

    class Server
    {
        private HttpListener listener;

        public Server() { }
        public void NewUser(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;
            DateTime curDate = DateTime.Now;
            char SeparatorChar = '&';
            Authentication Enter;

            var request = context.Request;
            var response = context.Response;
            using (StreamReader input = new StreamReader(
            request.InputStream, Encoding.UTF8))
            {
                string Command = null, Jstr = null;
                Program.Parser(input.ReadToEnd(), ref Command, ref Jstr, SeparatorChar);

                var connect = Connection("pavel6520.hopto.org", 25565, "project", "root", "6520");
                connect.Open();
                if (connect.State != ConnectionState.Open)
                {
                    Console.WriteLine("Ошибка подключения к базе данных");
                    return;
                }

                switch (Convert.ToInt32(Command))
                {
                    /////////////////Авторизация/////////////////
                    case 0:
                        Enter = JsonConvert.DeserializeObject<Authentication>(Jstr);

                        using (var Reader = Query(connect, "select inn from project.users where login = @login && password = @password", true,
                            new Query("@login", Double.PositiveInfinity, Enter.Login), new Query("@password", Double.PositiveInfinity, Enter.Password)))
                        {
                            if (Reader.HasRows)
                            {
                                SendMessage(response, "1");
                                Console.WriteLine(curDate + " Подключен пользовaтель " + Enter.Login);
                                Console.WriteLine();
                            }                                
                            else
                                SendMessage(response, "0");
                        }                        

                        break;

                    /////////////////Список INN-CompName/////////////////
                    case 1:
                        List<INN_Comp.Body_Element> tmp = new List<INN_Comp.Body_Element>();
                        var Login = JsonConvert.DeserializeObject<Authentication>(Jstr).Login;

                        using(var Reader = Query(connect, "select inn, comp from project.inn_comp where login = @login", true,
                            new Query("@login", Double.PositiveInfinity, Login)))
                        {
                            while (Reader.Read())
                            {
                                tmp.Add(new INN_Comp.Body_Element(Reader[0].ToString(), Reader[1].ToString()));
                            }
                                                    
                            SendMessage(response, JsonConvert.SerializeObject(new INN_Comp(tmp)));
                        }

                        break;

                    case 2:
                        Login = JsonConvert.DeserializeObject<Authentication>(Jstr).Login;

                        Console.WriteLine(curDate + " Пользовaтель " + Login + " отключен");
                        Console.WriteLine();

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

        }
        public void Stop()
        {
            // останавливаем прослушивание подключений 
            listener.Stop();
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
               + ";port=" + port + ";user=" + username + ";password=" + password); ;

        }        
        public MySqlDataReader Query(MySqlConnection connect, string query, bool need, params Query[] parametrs)
        {
            using (var command = new MySqlCommand(query, connect))
            {
                if (need)
                {
                    for (int i = 0; i < parametrs.Length; i++)
                    {
                        if (parametrs[i].parametr != Double.PositiveInfinity)
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr));
                        else
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr1));
                    }

                    return
                        command.ExecuteReader();

                }
                else
                {
                    for (int i = 0; i < parametrs.Length; i++)
                    {
                        if (parametrs[i].parametr != Double.PositiveInfinity)
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr));
                        else
                            command.Parameters.Add(new MySqlParameter(parametrs[i].name, parametrs[i].parametr1));
                    }

                    command.ExecuteReader();
                    return
                        null;
                }
            }
        }
    }
}
