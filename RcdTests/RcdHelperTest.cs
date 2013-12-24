using System.Linq;
using SopMessageView.Libs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace RcdTests
{
    
    
    /// <summary>
    ///这是 RcdHelperTest 的测试类，旨在
    ///包含所有 RcdHelperTest 单元测试
    ///</summary>
    [TestClass()]
    public class RcdHelperTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion
        [TestMethod]
        public void TestReplace()
        {
            string str = string.Empty;
            var temp= str.Replace("\0", "\\0");
            Assert.Fail();
        }

        [TestMethod]
        public void TestRcd()
        {
            string rcd = @"C:\spdbTrunk\Banks\Spdb\CommonFiles\MessageMapRcd\0106-基金对公客户信息查询.rcd";
            XDocument doc = XDocument.Load(rcd);
            var categories =
                doc.Descendants("Category").Where(o => o.Attribute("Name").Value == "Request").First().Descendants("Field").Where(o=>!o.Attribute("FieldLength").Value.Contains("var"));
            Assert.AreEqual(categories.Count(),28);
        }

        [TestMethod]
        public void TestReadMsg()
        {
            string rcd = @"G:\E809-轧帐检查账户配置.rcd";
            string msg = @"0000(0000)  00 CA 31 32 33 34 35 36 37 38 39 30 31 32 33 34   **12345678901234
0010(0016)  35 36 39 33 30 31 00 00 00 00 00 00 00 00 00 00   569301**********
0020(0032)  00 00 00 00 00 00 0A 70 13 6C 0A 70 12 3A 00 01   *******p*l*p*:**
0030(0048)  04 27 00 00 00 00 00 46 45 53 30 30 00 00 00 00   *'*****FES00****
0040(0064)  39 33 30 31 31 31 30 30 31 37 31 33 45 38 30 39   930111001713E809
0050(0080)  00 00 00 00 00 00 00 00 7E 00 00 00 00 46 45 53   ********}****FES
0060(0096)  30 30 32 34 31 32 39 39 30 32 30 31 33 31 32 31   0024129902013121
0070(0112)  38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   8***************
0080(0128)  00 00 00 00 00 00 00 00 00 00 00 00 01 49 00 00   *************I**
0090(0144)  00 00 00 04 39 33 30 31 00 00 00 00 06 46 45 38   ****9301*****FE8
00A0(0160)  30 39 31 01 0A 01 30 01 4F 01 31 01 31 11 39 33   091***0*O*1*1*93
00B0(0176)  30 31 30 31 33 33 31 35 30 30 30 30 30 31 38 04   010133150000018*
00C0(0192)  39 33 30 31 01 30 00 00 00 00                       9301*0***       ";
            var msgBytes = MainFrameTradeLogHelper.GetHexMessageFromLog(msg);
            var fields = RcdHelper.GetRequestMessageNodes(rcd,msgBytes);

            var count = fields.Count;
            Assert.AreEqual(count,28);
        }
    }
}
