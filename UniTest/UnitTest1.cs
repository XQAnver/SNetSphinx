using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SNetSphinx;
namespace UniTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           FrmMain form= new SNetSphinx.FrmMain();
           string url = "http://www.umei.cc/p/gaoqing/rihan/20150510173034.htm";
           form.Start(url);
        }
    }
}
