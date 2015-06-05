using SNS.DataUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SNS.ContnentAnalyse;
namespace SNS.SNetSphinx
{
    /// <summary>
    /// 爬虫的主要，搜索算法，以及线程资源的控制。
    /// </summary>
    public class Sphinx
    {
        
        public string url { get; set; }
        public delegate void Intresting( Object content,EventArgs e);
        public delegate void BadThing(Exception ex, object sender);
        public event Intresting OnGetIntreasting;
        public event BadThing OnBadThingHappened;
        WebPageInfo WebInfo = null;
        public  Sphinx()
        {
            WebInfo = new WebPageInfo();
        }
        public Sphinx(string url) :
            this()
        {
            this.url = url;
        }

        public void Run()
        {
            var UrlSphinx = new KeyValuePair<string, Sphinx>(this.url, this);
            try
            {
                WebInfo.SphinxManager.Add(UrlSphinx);
                 
                WebInfo.LableInfo.AddRange(HtmlAnalyse.GethttpLable(GetHtmlContent(url)));

                DealHtmlLable(WebInfo.LableInfo);
            }
            catch (Exception ex)
            {

                if (OnBadThingHappened != null)
                {
                    OnBadThingHappened(ex, this);
                }
            }
            finally{
                //WebInfo.SphinxManager.Remove(UrlSphinx);
                WebPageInfo.ClearSM(ref UrlSphinx);
            }
        }
        
        public void Run(object url)
        {
            try
            {
                string htmlContent = GetHtmlContent(url.ToString());
                WebInfo.LableInfo.AddRange(HtmlAnalyse.GethttpLable(htmlContent));
                DealHtmlLable(WebInfo.LableInfo);
            }
            catch (Exception ex)
            {

                if (OnBadThingHappened != null)
                {
                    OnBadThingHappened(ex, this);
                }
            }
            finally
            {
                try
                {
                    var UrlSphinx = WebInfo.SphinxManager.Find((item) =>
                    {
                        if (item.Key == this.url)
                            return true;
                        else
                            return false;
                    });
                   // WebInfo.SphinxManager.Remove(UrlSphinx);
                    WebPageInfo.ClearSM(ref UrlSphinx);
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }

        }

        private void GetImgContent(string url)
        {
            try
            {
                HttpWebResponse hwr = HttpUtility.HttpHelperWebResponse.CreateGetHttpResponse(url, 6000, null, null);
                Stream stream = hwr.GetResponseStream();
                if (OnGetIntreasting != null)
                    OnGetIntreasting(stream, new GetEventArgs(url));
            }
            catch (Exception ex)
            {

                if (OnBadThingHappened != null)
                {
                    OnBadThingHappened(ex, this);
                }
            }

        }

        private void DealHtmlLable(List<KeyValuePair<string, Dictionary<string, string>>> LableList)
        {
            int length = LableList.Count;
            for (int i = 0; i < length; i++)
            {
                KeyValuePair<string, Dictionary<string, string> >item= LableList[i];

                switch (item.Key.ToUpper())
                {
                    case "IMG":
                        GetHtmlImg(item.Value);
                        break;
                    case "A":
                        SphinexHtml(item.Value);
                        break;
                    case"SCRIPT":
                        //ContnentAnalyse.ScriptAnalyse.Analyse(item.Value);
                        break;
                    default:
                        break;
                }
                lock (WebInfo.LableInfo)
                {
                    WebInfo.LableInfo.Remove(item);
                    i--;
                }
                
            }
            
        }

        private void SphinexHtml(Dictionary<string, string> dictionary)
        {

           
            foreach (var item in dictionary)
            {
                switch (item.Key)
                {
                    case "href":    
                    case "src":
                        Sphinx sp = new Sphinx();
                        sp.OnBadThingHappened = this.OnBadThingHappened;
                        sp.OnGetIntreasting = this.OnGetIntreasting;
                        WaitCallback wCallFunc = new WaitCallback(sp.Run);
                        string value = HtmlAnalyse.DealLinkLable(url, item.Value);
                        if (!WebInfo.SphinxManager.Exists((machSphinx) => { return machSphinx.Key == value; }))
                        {
                            if (string.IsNullOrEmpty(value)) continue;
                            sp.url = value;
                            WebInfo.SphinxManager.Add(new KeyValuePair<string, Sphinx>(sp.url, sp));
                            ThreadPool.QueueUserWorkItem(wCallFunc, value);
                        }
                        break;

                    default:
                        break;
                }

            }
        }

        
        private void GetHtmlImg(Dictionary<string, string> dictionary)
        {
            WaitCallback wCallFunc=new WaitCallback (GetIMGContent);
            
            foreach (var item in dictionary)
            {
                switch (item.Key)
                {
                    case"src":
                        ThreadPool.QueueUserWorkItem(wCallFunc, item.Value);
                        break;
                        
                    default:
                        break;
                }

            }
        }

        private void GetIMGContent(object state)
        {
            try
            {
                string src = state as string;
                if (src.IndexOf("http://")<0)
                {
                    src = this.url + src;
                }
                HttpWebResponse hwr = HttpUtility.HttpHelperWebResponse.CreateGetHttpResponse(src, 100000, null, null);
                
                Stream stream=hwr.GetResponseStream();
                if (OnGetIntreasting!=null)
                {
                    OnGetIntreasting(stream,new GetEventArgs(src));
                }

            }
            catch (Exception ex)
            {

                if (OnBadThingHappened != null)
                {
                    OnBadThingHappened(ex, this);
                }
            }
        }

        private string  GetHtmlContent(string url)
        {
            HttpWebResponse hwr = HttpUtility.HttpHelperWebResponse.CreateGetHttpResponse(url, 10000, null, null);
            string encodingName = hwr.CharacterSet;
            byte[] resulteBytes = Stream2Bytes(hwr.GetResponseStream());
            string result=string.Empty;
            switch (encodingName.ToUpper())
            {
                case"UTF-8":
                    result = Encoding.UTF8.GetString(resulteBytes);
                    break;
                case"ISO-8859-1":
                    result=Encoding.ASCII.GetString(resulteBytes);
                break;
                default:
                   
                    break;
            }



            return result;
            //object htmlContent = null;
            //AutoResetEvent are = new AutoResetEvent(false);
            //HtmlDataUtility hduitility = new HtmlDataUtility();
            //hduitility.invokWindow = new System.Windows.Forms.Form();
            //hduitility.GetHtmlContent(url,(ob) =>
            //{
            //    htmlContent = ob;
            //    are.Set();
            //});

            //are.WaitOne();
            //return htmlContent.ToString ();
            

        }

        private byte[] Stream2Bytes(Stream s)
        {
            List<byte> Lbyte = new List<byte>();
            while (s.CanRead)
            {
                int n = s.ReadByte();
                // The end of the file is reached.
                if (n == -1)
                {
                    break;
                }
                Lbyte.Add((byte)n);

            }

           
            s.Flush();
            s.Dispose();
            s.Close();
            return Lbyte.ToArray();
        }

         
    }
}
