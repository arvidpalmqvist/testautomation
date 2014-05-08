using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NUnitTest.Utils
{
    static class WCFConn
    {
                
        private const string ENDPOINT = "http://localhost:1918/Service.svc/";


        public static List<Product> GetProducts()
        {
            string response = GetREST("GetAll");
            return Helper.GetProductListFromJSON(response);
        }

        public static void DeleteProduct(Product product)
        {
            DeleteREST("/Delete/" + product.ID.ToString());
        }

        private static string DeleteREST(string service)
        {
            WebRequest request = WebRequest.Create(ENDPOINT + service);
            request.Method = "DELETE";
            try
            {
                WebResponse response = request.GetResponse();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.Append(line).Append("\n");
                    }
                    return sb.ToString();
                }
            }
            catch (Exception x)
            {
                // TODO: Error handling
                throw x;
            }
        }

        private static string GetREST(string service)
        {
            WebRequest request = WebRequest.Create(ENDPOINT + service);
            
            try
            {
                WebResponse response = request.GetResponse();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.Append(line).Append("\n");
                    }
                    return sb.ToString();
                }
            }
            catch (Exception x)
            {
                // TODO: Error handling
                throw x;
            }
        }

        // Both Update and Insert
        public static string PostProduct(Product product)
        {
            return PostREST("/Post", Helper.GetJSONFromProduct(product));
        }

        public static string PostJson(String json) //Used to send unparseable json (negative tests)
        {
            return PostREST("/Post", json);
        }

        private static string PostREST(string service, string body)
        {

            WebRequest request = WebRequest.Create(ENDPOINT + service);
            request.Method = "POST";

            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            writer.Write(body);
            writer.Flush();
            writer.Close();
            
            try
            {
                WebResponse response = request.GetResponse();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.Append(line).Append("\n");
                    }
                    return sb.ToString();
                }        
            }
            catch (Exception x)
            {
                // TODO: Error handling
                throw x;
            }
 
        }


    }
}
