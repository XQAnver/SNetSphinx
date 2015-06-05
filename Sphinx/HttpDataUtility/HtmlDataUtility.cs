using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SNS.DataUtility
{
    /// <summary>
    /// HTML各个信息的下载，以及相关动作的提交等。
    /// </summary>
    public class HtmlDataUtility
    {

        public delegate void OnHtmlCompleteEventHandler(HtmlDocument content);
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="actComlete"></param>
        public void GetHtmlContent(string url, OnHtmlCompleteEventHandler actComlete)
        {
            GetHtmlContent(url,new WebBrowserDocumentCompletedEventHandler((ob,e)=>{
                WebBrowser wb = ob as WebBrowser;
                if (actComlete != null && wb!=null)
                {
                    actComlete(wb.Document);
                }
            }));
        }

        [STAThread]
        private void GetHtmlContent(string url, WebBrowserDocumentCompletedEventHandler actComlete)
        {

            invokWindow.Invoke(new Action(() =>
            {
                
               
                WebBrowser wbHelper = new WebBrowser();

                wbHelper.Url = new Uri(url, System.UriKind.Absolute);

                wbHelper.DocumentCompleted += actComlete;

            }), null);
        }

        public ISynchronizeInvoke invokWindow { get; set; }
    }
}
