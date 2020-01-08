using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPrint
{
    class Config
    {
        public static int G_READER_TIMER = 3000;
        public static int G_STATUS_TIMER = 3000;
        public static string G_DB_IP_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "cfg", "dbip", "127.0.0.1");
        public static string G_DB_DATABASE_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "cfg", "dbname", "unisky1");
        public static string G_DB_USER_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "cfg", "user", "root");
        public static string G_DB_PWD_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "cfg", "password", "qq1223");
        public static string G_NO = INIOperationClass.INIGetStringValue(".\\config.ini", "cfg", "NO", "1");
        public static string G_DB_CONN_STR = "server=" + G_DB_IP_STR + ";uid=" + G_DB_USER_STR + ";pwd=" + G_DB_PWD_STR + ";database=" + G_DB_DATABASE_STR + ";charset=gb2312;";
    }
}
