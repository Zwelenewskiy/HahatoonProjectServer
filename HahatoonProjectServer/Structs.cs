using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahatoonProjectServer
{
    public static class Structs
    {
        public static readonly string HOST = "http://localhost:8888/";
        public static readonly string CONNECTION_FILE = "Connection.txt";
        public static readonly string BD_NAME = "hakaton";
        public static readonly char SeparatorChar = '&';
        public static Server server = null;
        public static long usersCount;
        public static Hashtable users = new Hashtable();

        /// <summary>
        /// Представляет константы для типов ошибок
        /// </summary>
        public enum Message
        {
            ConnectionFileNotExists,
            ErrorCreatingConnection,
            ErrorStartServer,
            ErrorStopServer,
        }

        /// <summary>
        /// Данные для авторизации
        /// </summary>
        public struct Authentication
        {
            public string Login, Password;

            public Authentication(string log, string pass)
            {
                Login = log;
                Password = pass;
            }
        }

        public struct INN_Comp
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

        /// <summary>
        /// Данные отчета
        /// </summary>
        public struct Report
        {
            public string inn;
            public int quarter;
            public int year;

            public int[] param1;
            public int[] param2;
            public double[] param3;
        }
        
        public struct Query
        {
            public double? parametr;
            public string parametr1, name;

            public Query(string n, double? p, string p1)
            {
                name = n;
                parametr = p;
                parametr1 = p1;
            }
        }
    }
}
