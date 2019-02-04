using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahatoonProjectServer
{
    public static class Structs
    {
        public static readonly string HOST = "http://localhost:8888/";
        public static readonly char SeparatorChar = '&';

        public enum Errors
        {
            FileNotExists,
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

        /*public static void Connection()
        {
            if (File.Exists(CONNECTION_FILE))
            {

            }
            else
            {
                ShowError(1);
            }
        }
        
        public static void ShowError(Structs.Errors error)
        {
            switch (error)
            {
                case Structs.Errors.FileNotExists:
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(" File with connection settings");

                    break;
            }
        }     
         
         */
    }
}
