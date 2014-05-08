using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using NUnit.Framework;
using System.IO;
using System.Xml;
using NUnitTest.TestData;
using NUnitTest.Utils;

namespace NUnitTest.Setup
{

    [TestFixture()]
    abstract class TestBase : IDisposable
    {
        #region Variables
        protected IWebDriver Driver { get; set; }
        protected string CurrentBrowser { get; set; }
        
        private const string CHROME_DRIVER = @"C:\Users\Arvid Palmqvist\Desktop\.NET libraries\Selenium\Chrome";
        #endregion

        #region SetUp
        [SetUp()]
        public void Init()
        {
            DBConn.EmptyDatabase();
            DBConn.LoadDatabase();
        }
        #endregion

        #region SetDriver
        protected void SetDriver(string browser)
        {
            CurrentBrowser = browser;
            if (browser.Equals("Firefox")) Driver = new FirefoxDriver();
            if (browser.Equals("Chrome")) Driver = new ChromeDriver(CHROME_DRIVER);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
        }
        #endregion

        #region TearDown
        [TearDown()]
        public void Dispose()
        {
            if (Driver != null) Driver.Dispose();
        }
        #endregion


        public static void Main(String[] args)
        { }

    }

}
