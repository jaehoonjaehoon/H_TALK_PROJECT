using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GlobalVar
{
    class Var
    {
        public static string ReadIPandPortTextFilePath = "D:\\H_Talk_System\\";
        public static string ReadIPandPortTextFileName = "IPandPort.txt";

        public static int form_State = (int)FORM_STATE.MAIN;
        public enum FORM_STATE
        {
            MAIN = 0,
            SIGN_UP = 1
        }

    }
    class Client
    {
        public static string m_id;
        public static string m_password;
        public static string name;
        public static string eMail;
    }


}
