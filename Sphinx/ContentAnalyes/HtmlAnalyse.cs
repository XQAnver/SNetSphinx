using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SNS.ContnentAnalyse
{
    /// <summary>
    /// HTML内容的分析：路径的识别，以及HTML内的操作识别，内容的识别等。并分门别类的操作。
    /// </summary>
    public class HtmlAnalyse
    {
        public static List<KeyValuePair<string, Dictionary<string, string>>> GethttpLable(string htmlContent)
        {
            List<KeyValuePair<string, Dictionary<string, string>>> LableList = new List<KeyValuePair<string, Dictionary<string, string>>>();
            //获取形如<lble a=b>的标签
            Regex reg = new Regex(@"<(\w+)\s(?:(\w+)=""([^""]+)""\s{0,}|(\w+)='([^']+)'\s{0,}|(\w+)=([^\s>]+)\s{0,})+[^>]{0,}>", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            MatchCollection collection = reg.Matches(htmlContent);

            foreach (Match mitem in collection)
            {

                Dictionary<string, string> lblPropertise = new Dictionary<string, string>();
                string lblName = string.Empty;
                lblName = mitem.Groups[1].Captures[0].Value;
                for (int i = 2; i < mitem.Groups.Count; i++)
                {
                    string name = mitem.Groups[i].Captures.Count > 0 ? mitem.Groups[i].Captures[0].Value : "";
                    string value = string.Empty;

                    if (!string.IsNullOrEmpty(name))
                    {
                        for (int j = 0; j < mitem.Groups[i].Captures.Count; j++)
                        {
                            name = mitem.Groups[i].Captures[j].Value;
                            value = mitem.Groups[i + 1].Captures[j].Value;
                            lblPropertise.Add(name, value);
                        }
                        i++;
                    }
                }

                LableList.Add(new KeyValuePair<string, Dictionary<string, string>>(lblName, lblPropertise));
            }
            htmlContent = null;

            return LableList;
        }


        public static object DealHtmlLable(List<KeyValuePair<string, Dictionary<string, string>>> LableList)
        {
            return null;
        }


        public static string DealLinkLable(string url, string value)
        {
           
            value = value.Replace("#", "");
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            StringBuilder newUrl = new StringBuilder();
            if (value.IndexOf("http://") ==0)
                return value;
           
            else
            { 
                
                string[] urls = url.Substring(7).Split('/');
                //根地址
                string rootPath = urls[0];
                //当前目录地址
                string curLevelPath = url.Replace(urls[urls.Length - 1], "");
                
                //加入HTTP头
                newUrl.Append("http://");
                //相对地址为[/xx/xx/xx],绝对地址应为[根地址+相对地址]
                if (value.IndexOf("/") == 0)
                {
                    newUrl.Append (rootPath );
                    newUrl.Append(value);
                    return string.Empty;
                }
                //相对地址为[./xx/xx/xx],绝对地址应为[当前目录+相对地址]
                else if (value.IndexOf("./") == 0 || value.IndexOf("/") > 1)
                {
                    newUrl.Append(curLevelPath);
                    newUrl.Append(value.Replace("./", ""));
                    
                }
                //相对地址为[../xx/xx/xx],绝对地址为[父目录地址+相对地址]
                else if (value.IndexOf("../") > -1)
                {

                    int j = value.Split('.', '.', '/').Length + 1;
                    for (int i = 0; i < urls.Length - j; i++)
                    {
                        newUrl.Append(urls);
                    }
                    newUrl.Append("/");
                    newUrl.Append(value);
                    return string.Empty;
                }
                else if (value.IndexOf("/") > 1 || value.IndexOf("/") < 0)
                {
                    for (int i = 0; i < urls.Length-1; i++)
                    {
                        newUrl.Append(urls[i]);
                        newUrl.Append("/");
                    }
                   
                    newUrl.Append(value);
                }
                   
            }
    
            return  newUrl.ToString();;
            
        }
    }
}
