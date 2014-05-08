using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using NUnit.Framework;

namespace NUnitTest.TestData
{
    static class TestDataProvider
    {

        public static IEnumerable GetTestData()
        {
            foreach (string Browser in Properties.GetBrowsers())
            {
                yield return new TestCaseData(Browser).SetName(Browser);
            }
        }




        #region JUST TO REMEMBER HOW TO FEED TESTCASES WITH OTHER TESTDATA THAN BROWSERS
        //public IEnumerator GetEnumerator()
        //{
        //    XmlDocument Document = new XmlDocument();
        //    Document.Load(TESTDATA_XML_LOCATION);

        //    XmlNodeList Nodes = Document.DocumentElement.SelectNodes("/testData/testDataRow");

        //    foreach (string browser in Properties.GetBrowsers())
        //    {
        //        foreach (XmlNode Node in Nodes)
        //        {
        //            yield return new TestDataModel()
        //            {
        //                Browser = browser,
        //                Username = Node.SelectSingleNode("name").InnerText,
        //                Password = Node.SelectSingleNode("price").InnerText
        //            };
        //        }
        //    }

        //}
        //private const string TESTDATA_XML_LOCATION = @"C:\Users\Arvid Palmqvist\Desktop\backup\NUnitTest\NUnitTest\TestData\TestDataFile.xml";

        #endregion
        
    }
}
