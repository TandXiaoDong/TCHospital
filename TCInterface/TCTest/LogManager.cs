using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace TCTest
{
    class LogManager
    {
        string strDir = Directory.GetCurrentDirectory() + "\\log";//文件夹
        public void info(string content)
        {
            DateTime dt = DateTime.Now;
            string logName = dt.ToString("yyyy-MM-dd");

            string strFile = strDir + "\\" + logName + ".txt";//txt文件
            //string content = "ye111";//内容
            if (!Directory.Exists(strDir))
            {
                Directory.CreateDirectory(strDir);
            }
            if (!File.Exists(strFile))
            {
                File.WriteAllText(strFile, "");
            }

            StreamWriter FileWriter = new StreamWriter(strFile, true); //创建日志文件
            FileWriter.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss") + " " + content);
            FileWriter.Close(); //关闭StreamWriter对象
        }
    }
}
