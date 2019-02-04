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

        /*public static void ShowError(byte errorCode)
        {
            switch (errorCode)
            {
                case 1:
                    break;
            }
        }

        public static void Connection()
        {
            if (File.Exists(CONNECTION_FILE))
            {

            }
            else
            {
                ShowError(1);
            }
        }*/
    }
}
