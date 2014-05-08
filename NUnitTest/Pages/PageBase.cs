using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using NUnitTest.TestData;

namespace NUnitTest.Pages
{
    abstract class PageBase
    {
        protected IWebDriver Driver { get; set; }

        public PageBase(IWebDriver driver)
        {
            Driver = driver;
        }
    }
}
