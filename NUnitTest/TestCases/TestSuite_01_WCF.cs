using NUnit.Framework;
using NUnitTest.Setup;
using NUnitTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssertExtensions;

namespace NUnitTest.TestCases
{
    class TestSuite_01_WCF : TestBase
    {

        /*********************** POSITIVE TESTS *************************/

        // Read 

        // 1	Verifiera alla produkter 
        [Test]
        public void Test_01_VerifyAllProducts()
        {
            List<Product> Expected = DBConn.GetProducts();
            List<Product> Actual = WCFConn.GetProducts();

            ReflectionAssert.AreEqual(Expected, Actual); 
        }


        // Create

        // 2	Lägg till ny produkt
        [Test]
        public void Test_02_AddProduct()
        {
            int?[] IDsBeforeInsert = DBConn.GetProducts().Select(r => r.ID).ToArray();
            
            Product Product = new Product 
            {
                Name = "Korvboll",
                Price = 0
            };
       
            WCFConn.PostProduct(Product);
            int?[] NewID = DBConn.GetProducts().Select(r => r.ID).Except(IDsBeforeInsert).ToArray();

            Assert.AreEqual(1, NewID.Count()); // The difference should be 1. Only 1 new Product should have been inserted.
            Assert.AreEqual(Product.Name, DBConn.GetProduct((int)NewID[0]).Name);
            Assert.AreEqual(Product.Price, DBConn.GetProduct((int)NewID[0]).Price);
        }


        // Delete

        // 3	Ta bort produkt
        [Test]
        public void Test_03_DeleteProduct()
        {
            Product Product = this.GetProductToWorkWith();
            WCFConn.DeleteProduct(Product);
            Assert.IsNull(DBConn.GetProduct((int)Product.ID));
        }


        // Update

        // 4	Editera produkt
        [Test]
        public void Test_04_EditProduct()
        {
            Product Expected = this.GetProductToWorkWith();

            // Edit
            if (Expected.Name.Equals("NewName"))
            {
                Expected.Name = "OtherNewName";
            }
            else
            {
                Expected.Name = "NewName";
            }
            Random rnd = new Random();
            Expected.Price = rnd.Next(1, 500000);

            WCFConn.PostProduct(Expected);

            Product Actual = DBConn.GetProduct((int)Expected.ID);

            ReflectionAssert.AreEqual(Expected, Actual);
        }

        
        /*********************** NEGATIVE TESTS *************************/

        // Create

        // 5	Lägg till ny produkt, felaktigt namn (0 tecken)
        [Test]
        public void Test_05_AddProduct_IncorrectName_TooFewChars()
        {
            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(new Product
            {
                Name = "",
                Price = 100
            }));
            Assert.AreEqual(Expected, Actual);
        }

        // 6	Lägg till ny produkt, felaktigt namn (51 tecken)
        [Test]
        public void Test_06_AddProduct_IncorrectName_TooManyChars()
        {
            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(new Product
            {
                Name = "thisConsistsOfFiftyOneCharactersWhichIsOneTooMany!!",
                Price = 100
            }));
            Assert.AreEqual(Expected, Actual);
        }

        // 7	Lägg till ny produkt, felaktigt namn (otillåtet tecken)
        [Test]
        public void Test_07_AddProduct_IncorrectName_BadChar()
        {
            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(new Product
            {
                Name = "",
                Price = 100
            }));
            Assert.AreEqual(Expected, Actual);
        }
 
        // 8	Lägg till ny produkt, felaktigt namn (null)	
        [Test]
        public void Test_08_AddProduct_IncorrectName_Null()
        {
            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(new Product
            {
                Name = null,
                Price = 100
            }));
            Assert.AreEqual(Expected, Actual);
        }

        // 9	Lägg till ny produkt, felaktigt pris (-1)   
        [Test]
        public void Test_09_AddProduct_IncorrectPrice_TooLow()
        {
            string Expected = "Could not validate Price.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(new Product
            {
                Name = "SomeName",
                Price = -1
            }));
            Assert.AreEqual(Expected, Actual);
        }

        // 10	Lägg till ny produkt, felaktigt pris (500001)	
        [Test]
        public void Test_10_AddProduct_IncorrectPrice_TooHigh()
        {
            string Expected = "Could not validate Price.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(new Product
            {
                Name = "SomeName",
                Price = 500001
            }));
            Assert.AreEqual(Expected, Actual);
        }

        // 11	Lägg till ny produkt, felaktigt pris (otillåtet tecken)
	    [Test]
        public void Test_11_AddProduct_IncorrectPrice_BadChar()
        {
            string Expected = "Could not serialize Stream.";
            String json = "{\"ID\":null,\"Name\":\"SomeName\",\"Price\":\"PriceAsString\"}";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostJson(json));
            Assert.AreEqual(Expected, Actual);
        }

        // 12	Lägg till ny produkt, felaktigt pris (null)	   
        [Test]
        public void Test_12_AddProduct_IncorrectPrice_Null()
        {
            string Expected = "Could not serialize Stream.";
            String json = "{\"ID\":null,\"Name\":\"SomeName\",\"Price\":null}";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostJson(json));
            Assert.AreEqual(Expected, Actual);
        }

        // Edit

        // 13	Editera produkt, felaktigt namn (0 tecken)   
        [Test]
        public void Test_13_EditProduct_IncorrectName_TooFewChars()
        {
            Product Product = this.GetProductToWorkWith();

            // Edit
            Product.Name = "";

            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(Product));
            Assert.AreEqual(Expected, Actual);
        }

        // 14	Editera produkt, felaktigt namn (51 tecken)	  
        [Test]
        public void Test_14_EditProduct_IncorrectName_TooManyChars()
        {
            Product Product = this.GetProductToWorkWith();

            // Edit
            Product.Name = "thisConsistsOfFiftyOneCharactersWhichIsOneTooMany!!";

            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(Product));
            Assert.AreEqual(Expected, Actual);
        }

        // 15	Editera produkt, felaktigt namn (otillåtet tecken)
	    [Test]
        public void Test_15_EditProduct_IncorrectName_BadChar()
        {
            Product Product = this.GetProductToWorkWith();

            // Edit
            Product.Name = " Test";

            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(Product));
            Assert.AreEqual(Expected, Actual);
        }

        // 16	Editera produkt, felaktigt namn (null)	   
        [Test]
        public void Test_16_EditProduct_IncorrectName_Null()
        {
            Product Product = this.GetProductToWorkWith();

            // Edit
            Product.Name = null;

            string Expected = "Could not validate Name.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(Product));
            Assert.AreEqual(Expected, Actual);
        }

        // 17	Editera produkt, felaktigt pris (-1)	
        [Test]
        public void Test_17_EditProduct_IncorrectPrice_TooLow()
        {
            Product Product = this.GetProductToWorkWith();

            // Edit
            Product.Price = -1;

            string Expected = "Could not validate Price.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(Product));
            Assert.AreEqual(Expected, Actual);
        }

        // 18	Editera produkt, felaktigt pris (500001)	
        [Test]
        public void Test_18_EditProduct_IncorrectPrice_TooHigh()
        {
            Product Product = this.GetProductToWorkWith();

            // Edit
            Product.Price = 500001;

            string Expected = "Could not validate Price.";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostProduct(Product));
            Assert.AreEqual(Expected, Actual);
        }

        // 19	Editera produkt, felaktigt pris (otillåtet tecken)	
        [Test]
        public void Test_19_EditProduct_IncorrectPrice_BadChar()
        {
            Product Product = this.GetProductToWorkWith();

            string Expected = "Could not serialize Stream.";
            String json = "{\"ID\":"+Product.ID+",\"Name\":\"SomeName\",\"Price\":\"PriceAsString\"}";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostJson(json));
            Assert.AreEqual(Expected, Actual);
        }

        // 20	Editera produkt, felaktigt pris (null)	 
        [Test]
        public void Test_20_EditProduct_IncorrectPrice_Null()
        {
            Product Product = this.GetProductToWorkWith();

            string Expected = "Could not serialize Stream.";
            String json = "{\"ID\":" + Product.ID + ",\"Name\":\"SomeName\",\"Price\":null}";
            string Actual = Helper.GetErrorMessageFromJSON(WCFConn.PostJson(json));
            Assert.AreEqual(Expected, Actual);
        }

        private Product GetProductToWorkWith()
        {
            // Creates a Product if the DB is empty
            // Returns the first Product in the DB

            List<Product> Products = DBConn.GetProducts();

            if (Products.Count == 0)
            {
                WCFConn.PostProduct(new Product
                {
                    Name = "ToBeWorkedWith",
                    Price = 1234
                });
                Products = DBConn.GetProducts(); // Should be 1 
            }
            return Products[0];
        }

    }
}
