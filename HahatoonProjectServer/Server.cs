using System;
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

    class Server
    {
        private HttpListener listener;

        public Server() { }
        public void NewUser(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;
            DateTime curDate = DateTime.Now;
            char SeparatorChar = '&';

            var request = context.Request;
            var response = context.Response;
            using (StreamReader input = new StreamReader(
            request.InputStream, Encoding.UTF8))
            {            
                string Command = null, Jstr = null;
                Program.Parser(input.ReadToEnd(), ref Command, ref Jstr, SeparatorChar);

                //Report UserReport = JsonConvert.DeserializeObject<Report>(Jstr);                   

                var connect = Connection("pavel6520.hopto.org", 25565, "project", "root", "6520");
                connect.Open();
                if (connect.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Ошибка подключения к базе данных");
                    return;
                }

                switch (Convert.ToInt32(Command))
                {
                    case 0:
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
        public MySqlDataReader Query(MySqlConnection connect, string query, bool need)
        {
            if (need)
                return new MySqlCommand(query, connect).ExecuteReader();
            else
            {
                new MySqlCommand(query, connect).ExecuteReader();
                return null;
            }
        }
    }
}
