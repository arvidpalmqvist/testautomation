using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnitTest.TestData
{
    static class Properties
    {
        private const bool FIREFOX = true;
        private const bool CHROME = true;

        public static List<string> GetBrowsers()
        {
            List<string> Browsers = new List<String>();
            if (FIREFOX) Browsers.Add("Firefox");
            if (CHROME) Browsers.Add("Chrome");
            return Browsers;
        }
    }
}
