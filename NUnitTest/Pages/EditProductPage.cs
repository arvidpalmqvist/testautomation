﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnitTest.Pages
{
    class EditProductPage : PageBase
    {
        public EditProductPage(IWebDriver driver)
            : base(driver)
        { 
        }

        public string GetName()
        {
            return Driver.FindElement(By.Id("Name")).GetAttribute("value").ToString();
        }

        public string GetPrice()
        {
            return Driver.FindElement(By.Id("Price")).GetAttribute("value").ToString();
        }

        public void EditName(string name)
        {
            IWebElement InputNameField = Driver.FindElement(By.Id("Name"));
            InputNameField.Clear();
            InputNameField.SendKeys(name);
        }

        public void EditPrice(string price)
        {
            IWebElement InputNameField = Driver.FindElement(By.Id("Price"));
            InputNameField.Clear();
            InputNameField.SendKeys(price);
        }

        public ProductRegistryPage ClickSave()
        {
            Driver.FindElement(By.Id("Save")).Click();
            return new ProductRegistryPage(Driver);
        }

        public IEnumerable<string> GetErrors()
        {
            IEnumerable<IWebElement> Errors = Driver.FindElements(By.ClassName("field-validation-error"));
            foreach (IWebElement Error in Errors)
            {
                yield return Error.Text.ToString();
            }
        }

        public bool IsError()
        {
            IWebElement test = Driver.FindElement(By.CssSelector("input:invalid"));
            if (test == null) return false; else return true;
        }
    }
}