using NUnit.Framework;
using NUnitTest.Setup;
using NUnitTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssertExtensions;
using NUnitTest.TestData;
using NUnitTest.Pages;

namespace NUnitTest.TestCases
{
    class TestSuite_02_GUI : TestBase
    {

        private AddProductPage AddProductBase(string name, string price)
        {
            ProductRegistryPage ProductRegistryPage = new ProductRegistryPage(Driver);
            ProductRegistryPage.Open();
            AddProductPage AddProductPage = ProductRegistryPage.ClickAddNewProduct();
            AddProductPage.InputName(name);
            AddProductPage.InputPrice(price);
            return AddProductPage;
        }

        private EditProductPage EditProductBase(string name, string price)
        {
            ProductRegistryPage ProductRegistryPage = new ProductRegistryPage(Driver);
            ProductRegistryPage.Open();
            Product Product = ProductRegistryPage.GetNewestProduct();
            EditProductPage EditProductPage = ProductRegistryPage.ClickEditProduct(Product);
            EditProductPage.EditName(name);
            EditProductPage.EditPrice(price);
            return EditProductPage;
        }


        /*********************** POSITIVE TESTS *************************/
    
        // Read

        // 21	Verifiera alla produkter 
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_21_VerifyAllProducts(string browser)
        {
            List<Product> Expected = DBConn.GetProducts();

            SetDriver(browser);
            ProductRegistryPage ProductRegistryPage = new ProductRegistryPage(Driver);
            ProductRegistryPage.Open();
            List<Product> Actual = ProductRegistryPage.GetAllProducts().ToList();

            Assert.AreEqual(Expected.Count, Actual.Count);

            // Can't use ReflectionAssert since the GUI doesn't know about the ID of the Product
            int i = 0;
            foreach (Product Product in Expected)
            {
                Assert.AreEqual(Product.Name, Actual[i].Name);
                Assert.AreEqual(Product.Price, Actual[i].Price);
                i++;
            }
        }

        // Create

        // 22	Lägg till ny produkt  
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_22_AddProduct(string browser)
        {
            string Name = "PositiveInsertTest";
            string Price = "100";

            List<Product> ProductsBeforeInsert = DBConn.GetProducts();

            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase(Name, Price);
            ProductRegistryPage ProductRegistryPage = AddProductPage.ClickSave();

            Product Product = ProductRegistryPage.GetNewestProduct();

            Assert.AreEqual(ProductsBeforeInsert.Count + 1, DBConn.GetProducts().Count);
            Assert.AreEqual(Name, Product.Name);
            Assert.AreEqual(Price, Product.Price.ToString());
        }

        // Delete
 
        // 23	Ta bort produkt   
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_23_DeleteProduct(string browser)
        {
            List<Product> ProductsBeforeDelete = DBConn.GetProducts();

            SetDriver(browser);
            ProductRegistryPage ProductRegistryPage = new ProductRegistryPage(Driver);
            ProductRegistryPage.Open();
            ProductRegistryPage.DeleteProduct(ProductsBeforeDelete[0]);

            Assert.AreEqual(ProductsBeforeDelete.Count - 1, DBConn.GetProducts().Count);

            List<string> Names = ProductsBeforeDelete.Select(r => r.Name).ToList();
            List<int> Prices = ProductsBeforeDelete.Select(r => r.Price).ToList();

            foreach (Product Product in ProductRegistryPage.GetAllProducts())
            {
                Assert.Contains(Product.Name, Names);
                Assert.Contains(Product.Price, Prices);
            }

        }

        // Update

        // 24	Editera produkt 
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_24_EditProduct(string browser)
        {
            List<Product> Products = DBConn.GetProducts();

            string Name = "PositiveTestEdit";
            string Price = "4444";

            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase(Name, Price);
            ProductRegistryPage ProductRegistryPage = EditProductPage.ClickSave();

            Assert.AreEqual(Products.Count, DBConn.GetProducts().Count);
            Assert.AreEqual(Name, ProductRegistryPage.GetNewestProduct().Name);
            Assert.AreEqual(Price, ProductRegistryPage.GetNewestProduct().Price.ToString());
        }


        /*********************** NEGATIVE TESTS *************************/

        // Create

        // 25	Lägg till ny produkt, felaktigt namn (0 tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_25_AddProduct_IncorrectName_TooFewChars(string browser)
        {
            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase("", "100");
            AddProductPage.ClickSave();
            List<string> Errors = AddProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The Name field is required.", Errors[0]);
        }

        // 26	Lägg till ny produkt, felaktigt namn (51 tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_26_AddProduct_IncorrectName_TooManyChars(string browser)
        {
            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase("thisConsistsOfFiftyOneCharactersWhichIsOneTooMany!!", "100");
            AddProductPage.ClickSave();
            List<string> Errors = AddProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The field Name must be a string with a maximum length of 50.", Errors[0]);
        }

        // 27	Lägg till ny produkt, felaktigt namn (otillåtet tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_27_AddProduct_IncorrectName_BadChar(string browser)
        {
            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase(" ", "100");
            AddProductPage.ClickSave();
            List<string> Errors = AddProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The Name field is required.", Errors[0]);
        }

        // 28	Lägg till ny produkt, felaktigt pris (-1)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_28_AddProduct_IncorrectPrice_TooLow(string browser)
        {
            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase("OkName", "-1");
            AddProductPage.ClickSave();
            List<string> Errors = AddProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The field Price must be between 0 and 500000.", Errors[0]);
        }

        // 29	Lägg till ny produkt, felaktigt pris (500001)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_29_AddProduct_IncorrectPrice_TooHigh(string browser)
        {
            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase("OkName", "500001");
            AddProductPage.ClickSave();
            List<string> Errors = AddProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The field Price must be between 0 and 500000.", Errors[0]);
        }

        // 30	Lägg till ny produkt, felaktigt pris (otillåtet tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_30_AddProduct_IncorrectPrice_BadChar(string browser)
        {
            string BadChars = "BadCharacters";
            SetDriver(browser);
            AddProductPage AddProductPage = this.AddProductBase("OkName", BadChars);
            AddProductPage.ClickSave();

            if (CurrentBrowser.Equals("Chrome"))
            {
                // HTML5 Validation error message
                Assert.IsTrue(AddProductPage.IsError());
            }
            else
            {
                List<string> Errors = AddProductPage.GetErrors().ToList();
                Assert.AreEqual(1, Errors.Count); // Should be only 1 error
                Assert.AreEqual("The value '" + BadChars + "' is not valid for Price.", Errors[0]);
            }
        }


        // Update

        // 31	Editera produkt, felaktigt namn (0 tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_31_EditProduct_IncorrectName_TooFewChars(string browser)
        {
            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase("", "500000");
            EditProductPage.ClickSave();
            List<string> Errors = EditProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The Name field is required.", Errors[0]);
        }

        // 32	Editera produkt, felaktigt namn (51 tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_32_EditProduct_IncorrectName_TooManyChars(string browser)
        {
            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase("thisConsistsOfFiftyOneCharactersWhichIsOneTooMany!!", "0");
            EditProductPage.ClickSave();
            List<string> Errors = EditProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The field Name must be a string with a maximum length of 50.", Errors[0]);
        }

        // 33	Editera produkt, felaktigt namn (otillåtet tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_33_EditProduct_IncorrectName_BadChar(string browser)
        {
            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase(" ", "0");
            EditProductPage.ClickSave();
            List<string> Errors = EditProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The Name field is required.", Errors[0]);
        }

        // 34	Editera produkt, felaktigt pris (-1)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_34_EditProduct_IncorrectPrice_TooLow(string browser)
        {
            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase("OkName", "-1");
            EditProductPage.ClickSave();
            List<string> Errors = EditProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The field Price must be between 0 and 500000.", Errors[0]);
        }

        // 35	Editera produkt, felaktigt pris (500001)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_35_EditProduct_IncorrectPrice_TooHigh(string browser)
        {
            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase("OkName", "500001");
            EditProductPage.ClickSave();
            List<string> Errors = EditProductPage.GetErrors().ToList();

            Assert.AreEqual(1, Errors.Count); // Should be only 1 error
            Assert.AreEqual("The field Price must be between 0 and 500000.", Errors[0]);
        }

        // 36	Editera produkt, felaktigt pris (otillåtet tecken)
        [Test, TestCaseSource(typeof(TestDataProvider), "GetTestData")]
        public void Test_36_EditProduct_IncorrectPrice_BadChar(string browser)
        {
            string BadChars = "BadCharacters";
            SetDriver(browser);
            EditProductPage EditProductPage = this.EditProductBase("OkName", BadChars);
            EditProductPage.ClickSave();

            if (CurrentBrowser.Equals("Chrome"))
            {
                // HTML5 Validation error message
                Assert.IsTrue(EditProductPage.IsError());
            }
            else
            {
                List<string> Errors = EditProductPage.GetErrors().ToList();
                Assert.AreEqual(1, Errors.Count); // Should be only 1 error
                Assert.AreEqual("The value '" + BadChars + "' is not valid for Price.", Errors[0]);
            }
        }

    }
}
