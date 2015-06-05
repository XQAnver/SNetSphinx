using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SNS.SNetSphinx
{
    /// <summary>
    /// 网页信息管理
    /// </summary>
    public class WebPageInfo
    {
        List<KeyValuePair<string, Dictionary<string, string>>> _LableInfo = null;
        static List<KeyValuePair<string, Sphinx>> sphinxManager = null;

        /// <summary>
        /// 简单的网页内容存储
        /// </summary>
        public List<KeyValuePair<string, Sphinx>> SphinxManager
        {

            get
            {
                if (sphinxManager == null)
                    sphinxManager = new List<KeyValuePair<string, Sphinx>>();

                return sphinxManager;
            }
        }
        public WebPageInfo()
        {
            _LableInfo = new List<KeyValuePair<string, Dictionary<string, string>>>();
        }
        
        public List<KeyValuePair<string, Dictionary<string, string>>> LableInfo
        {
            get
            {
                if (_LableInfo == null)
                    _LableInfo = new List<KeyValuePair<string, Dictionary<string, string>>>();

                return _LableInfo;
            }
        }




         

        internal static void ClearSM(ref KeyValuePair<string, Sphinx> UrlSphinx)
        {
            lock (WebPageInfo.sphinxManager)
            {
                WebPageInfo.sphinxManager.Remove(UrlSphinx);
                WebPageInfo.sphinxManager.Add(new KeyValuePair<string, Sphinx>(UrlSphinx.Key, null));
            }
        }
    }

    public class GetEventArgs : EventArgs
    {
       
        public string SRC { get; set; }
        // 摘要:
        //     初始化 System.EventArgs 类的新实例。
        public GetEventArgs( string src)
        {
            SRC=src;
        }
    }
}
