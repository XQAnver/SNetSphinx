using SNS.SNetSphinx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SNetSphinx
{
    public partial class FrmMain : Form
    {

        Sphinx spider; 
        Action<Stream, object> callback = null;
        List<Thread> threads = new List<Thread>();
        public FrmMain()
        {
            InitializeComponent();
            spider = new Sphinx(textBox1.Text);
            spider.OnGetIntreasting += spider_OnGetIntreasting;
            spider.OnBadThingHappened += spider_OnBadThingHappened;
            callback = new Action<Stream,object>(DoStrem);
            new Action(ThreadManagerment).BeginInvoke(null, null);
        }

        void spider_OnBadThingHappened(Exception ex, object sender)
        {
            WebException e = ex as WebException;
            if (e != null)
            {
                if (e.Response != null)
                    Console.WriteLine(e.Message + ":" + e.Response.ResponseUri.ToString());
                else
                    Console.WriteLine(ex.Message + ":" + (sender as Sphinx).url);
            }
            else
            {
                Console.WriteLine(ex.Message);
            }
            //richTextBox1.Invoke(new Action(() =>
            //{
            //    if (e == null)
            //    {
            //        richTextBox1.Text += ex.Message + ":" + (sender as Sphinx).url;
            //    }
            //    else
            //    {
            //        if(e.Response!=null)
            //            richTextBox1.Text += e.Message + ":" +e.Response.ResponseUri.ToString();
            //        else
            //            richTextBox1.Text += ex.Message + ":" + (sender as Sphinx).url;
            //    }
            //}), null);

        }

        public void buttonStart_Click(object sender, EventArgs e)
        {
            string parameter = textBox1.Text;
            if (string.IsNullOrEmpty(parameter))
                return;
            Thread th=new Thread(new ParameterizedThreadStart(Start));
            lock (threads)
            {
                threads.Add(th); 
            }
           
            th.Start(parameter);
            buttonStart.Enabled = false;
        }

        public void Start(object obj)
        {
 
           spider.url = obj.ToString();
           spider.Run();

           //buttonStart.Enabled = true;
        }

        void spider_OnGetIntreasting(Object content, EventArgs ea)
        {

            //callback.BeginInvoke(content, (e as GetEventArgs).SRC, null, callback);
            DoStrem(content, (ea as GetEventArgs).SRC);
           
            richTextBox1.Invoke(new Action(() =>
            {
                if (ea != null)
                {
                    //richTextBox1.Text += (ea as GetEventArgs).SRC+"\r\n";
                }
            }), null);
        }

        void DoStrem(object arg1, object arg2)
        {
            if (arg1 != null)
            {
                Stream stream = arg1 as Stream;
                try
                {
                    
                    string src = arg2 as string;
                    string fileName = src.Substring(src.LastIndexOf("/") + 1);
                    fileName = System.Web.HttpUtility.UrlDecode(fileName);
                    string directory = src.Substring(0, src.LastIndexOf("/"));
                    directory = directory.Substring(directory.LastIndexOf("/") + 1);
                    string exfileName = src.Substring(src.LastIndexOf(".") + 1);
                    //string tempPath = System.Environment.CurrentDirectory + "/IMG/" + src.Remove(src.LastIndexOf("/")).Replace("http://","").Replace("/","%");

                    string tempPath = System.Environment.CurrentDirectory + @"\IMG\" + directory+@"\";
                 

                    Bitmap bmap = new Bitmap(stream);

                     
                    if (bmap.Width < 300 || bmap.Height < 300)
                        return;
                    //if (bmap.Width < 150 || bmap.Height < 150)
                    //    return;
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    //if (File.Exists(tempPath + "/" + fileName))
                    //    fileName = tempPath + "/~" + fileName;
                    //else
                    //    fileName = tempPath + "/" + fileName;

                    //fileName = tempPath + DateTime.Now.ToBinary() + fileName;
                    fileName = tempPath + fileName;
                    switch (exfileName.ToUpper())
                    {
                        case "PNG":
                            bmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case "JPG":
                        case "JPEG":
                            bmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case "BMP":
                            bmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case "GIF":
                            bmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        default:
                            break;
                    }

                   
                }
                catch (Exception)
                {


                }
                finally
                {
                    stream.Flush(); 
                    stream.Dispose();
                    stream.Close();
                }


            }
        }

        void ThreadManagerment()
        {
            List<Thread> stopThread = new List<Thread>();
            while (true)
            {
                if (threads.Count == 0)
                    Thread.Sleep(10000);
                else
                {
                    lock (threads)
                    {

                        foreach (Thread thitm in threads)
                        {
                            try
                            {
                                switch (thitm.ThreadState)
                                {
                                    case ThreadState.AbortRequested:
                                        break;
                                    case ThreadState.Aborted:
                                        break;
                                    case ThreadState.Background:
                                        break;
                                    case ThreadState.Running:
                                        //Console.WriteLine(thitm.ManagedThreadId + ":Running");
                                        break;
                                    case ThreadState.StopRequested:
                                        break;
                                    case ThreadState.Stopped:
                                        Console.WriteLine(thitm.ManagedThreadId + ":Stopped");
                                        stopThread.Add(thitm);
                                        break;
                                    case ThreadState.SuspendRequested:
                                        break;
                                    case ThreadState.Suspended:
                                        break;
                                    case ThreadState.Unstarted:
                                        Console.WriteLine(thitm.ManagedThreadId + ":Unstarted");
                                        break;
                                    case ThreadState.WaitSleepJoin:
                                        break;
                                    default:
                                        break;
                                }

                            }

                            catch (Exception)
                            {


                            }
                        }
                        foreach (var item in stopThread)
                        {
                            threads.Remove(item);
                        }
                        stopThread.Clear();
                    }
                }
            }
        }
    }
}
