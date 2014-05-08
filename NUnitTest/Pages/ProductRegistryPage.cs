using NUnitTest.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnitTest.Pages
{
    class ProductRegistryPage : PageBase
    {
        public ProductRegistryPage(IWebDriver driver)
            : base(driver)
        {
        }

        public void Open()
        {
            Driver.Navigate().GoToUrl("http://localhost:1925/");
        }

        // Method not used at the moment
        private IWebElement WaitForTable()
        {
            IWebElement Table = null;
            while (Table == null)
            {
                try
                {
                    WebDriverWait Wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 3));
                    Wait.Until(ExpectedConditions.ElementExists(By.Id("ProductRegistry")));
                    Table = Driver.FindElement(By.Id("ProductRegistry"));
                }
                catch (Exception ignored)
                {
                }
            }
            return Table;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            IWebElement Table = Driver.FindElement(By.Id("ProductRegistry"));
            IEnumerable<IWebElement> Products = Table.FindElements(By.ClassName("Product"));
            foreach (IWebElement Row in Products)
            {
                yield return new Product() 
                {
                    Name = Row.FindElement(By.ClassName("Name")).Text.ToString(),
                    Price = int.Parse(Row.FindElement(By.ClassName("Price")).Text.ToString())
                };
            }
        }

        public Product GetNewestProduct()
        {
            IEnumerable<Product> Products = this.GetAllProducts();
            return Products.Last();
        }

        public AddProductPage ClickAddNewProduct()
        {
            Driver.FindElement(By.Id("New")).Click();
            return new AddProductPage(Driver);
        }

        public EditProductPage ClickEditProduct(Product product)
        {
            IWebElement Table = Driver.FindElement(By.Id("ProductRegistry"));
            IEnumerable<IWebElement> Products = Table.FindElements(By.ClassName("Product"));
            foreach (IWebElement Row in Products)
            {
                if (Row.FindElement(By.ClassName("Name")).Text.ToString().Equals(product.Name))
                {
                    Row.FindElement(By.ClassName("Edit")).Click();
                    return new EditProductPage(Driver);
                }
            }
            return null;
        }

        public void DeleteProduct(Product product)
        {
            IWebElement Table = Driver.FindElement(By.Id("ProductRegistry"));
            IEnumerable<IWebElement> Products = Table.FindElements(By.ClassName("Product"));
            foreach (IWebElement Row in Products)
            {
                try
                {
                    if (Row.FindElement(By.ClassName("Name")).Text.ToString().Equals(product.Name))
                    {
                        Row.FindElement(By.ClassName("Delete")).Click();
                    }
                }
                catch (StaleElementReferenceException Ignore)
                { 
                }
            }   
        }
    }
}
