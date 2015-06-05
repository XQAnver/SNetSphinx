using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Configuration;

namespace Log
{
     public static class LogCat
    {
         const string  LOGNAME="";
         public static bool CONSOLEONLY = false;
         private static object _lockWrite = new object();
         private static object _lockConsol = new object();
         public static void e(string TAG, string msg)
         {
             string messag = string.Format("[e]\t时间：{0}，来源：{1}，消息：{2}", DateTime.Now, TAG, msg);
             WriteLog(messag);
             Output(messag);
         }
         public static void e(string msg)
         {
             string messag = string.Format("[e]\t时间：{0}，消息：{1}", DateTime.Now, msg);
             WriteLog(messag);
             Output(messag);
         }

         public static void w(string TAG, string msg)
         {
             string messag = string.Format("[w]\t时间：{0}，来源：{1}，消息：{2}", DateTime.Now, TAG, msg);
             WriteLog(messag);
             Output(messag);
         }
         public static void w(string msg)
         {
             string messag = string.Format("[w]\t时间：{0}，消息：{1}", DateTime.Now, msg);
             WriteLog(messag);
             Output(messag);
         }

         public static void i(string TAG, string msg)
         {
             string messag = string.Format("[i]\t时间：{0}，来源：{1}，消息：{2}", DateTime.Now, TAG, msg);
             WriteLog(messag);
             Output(messag);
         }
         public static void i(string msg)
         {
             string messag = string.Format("[i]\t时间：{0}，消息：{1}", DateTime.Now, msg);
             WriteLog(messag);
             Output(messag);
         }

         public static void d(string TAG, string msg)
         {
             string messag = string.Format("[d]\t时间：{0}，来源：{1}，消息：{2}", DateTime.Now, TAG, msg);
             WriteLog(messag);
             Output(messag);
         }
          public static void d(string msg)
         {
             string messag = string.Format("[d]\t时间：{0}，消息：{1}", DateTime.Now, msg);
             WriteLog(messag);
             Output(messag);
         }

         /// <summary>
         /// 写日志
         /// </summary>
         /// <param name="error"></param>
         public static void WriteLog(string error)
         {
             if (!CONSOLEONLY)
             {
                 lock (_lockWrite)
                 {
                     //string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\ErrorLog.txt";
                     string path = string.Empty ;
                     if(ConfigurationManager.AppSettings["ErrorLogPath"] == null){
                          path = Path.GetDirectoryName(Application.ExecutablePath) + "\\ErrorLog.txt";
                     }
                     else{
                          path = ConfigurationManager.AppSettings["ErrorLogPath"];
                     }
                     try
                     {
                         System.IO.StreamWriter w = new StreamWriter(path, true, Encoding.GetEncoding("gb2312"));
                         using (w)
                         {
                             //w.WriteLine(DateTime.Now.ToString());
                             w.WriteLine(error);
                             w.Close();
                         }
                     }
                     catch (Exception)
                     {
                         //没法写日志
                     }
                   
                 }
             }
         }


         private static void Output(string error)
         {
             lock (_lockConsol)
             {
                 if (CONSOLEONLY)
                 {
                     System.Console.WriteLine(error);
                 }
                 
             }
            
         }
    }
}
