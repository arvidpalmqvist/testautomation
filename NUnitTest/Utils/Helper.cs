using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnitTest.Utils
{
    static class Helper
    {
        public static List<Product> GetProductListFromJSON(string json)
        {
            try
            {
                RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
                List<Product> list = JsonConvert.DeserializeObject<List<Product>>(root.ProductsResult);
                return list;
            }
            catch (Exception x)
            {
                // TODO: Error handling
                throw x;
            }
        }

        public static Product GetProductFromJSON(string json)
        {
            try
            {
                RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
                Product Product = JsonConvert.DeserializeObject<Product>(root.ProductResult);
                return Product;
            }
            catch (Exception x)
            {
                // TODO: Error handling
                throw x;
            }
        }

        public static string GetErrorMessageFromJSON(string json)
        {
            try
            {
                RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
                return root.HandleMessageResult;
            }
            catch (Exception x)
            {
                return "Could not parse an error message from the JSON received.";
            }
        }

        public static string GetJSONFromProduct(Product obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
