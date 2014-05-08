using NUnitTest.TestData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;

namespace NUnitTest.Utils
{
    static class DBConn
    {
        private const string DB_CONN_DETAILS = "Data Source=WIN-N9FVFFLN540\\ARVIDSMSSQL;Initial Catalog=Produktregistret;User Id=Arvid Palmqvist;Password=;Trusted_Connection=Yes;";
        private const string BASEDATA_XML_LOCATION = @"C:\Users\Arvid Palmqvist\Desktop\backup\NUnitTest\NUnitTest\TestData\BaseDataFile.xml";

        public static void EmptyDatabase()
        {
            using (SqlConnection conn = new SqlConnection(DB_CONN_DETAILS))
            {
                try
                {
                    conn.Open();
                    string delete = "DELETE FROM Product";

                    using (SqlDataAdapter Delete = new SqlDataAdapter())
                    {
                        Delete.SelectCommand = new SqlCommand(delete, conn);
                        Delete.SelectCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static void LoadDatabase()
        {

            foreach (Product Product in GetBaseData().ToList())
            {
                using (SqlConnection conn = new SqlConnection(DB_CONN_DETAILS))
                {
                    try
                    {
                        conn.Open();
                        string insert = "INSERT INTO Product (Name, Price) VALUES (@Name, @Price)";

                        using (SqlDataAdapter Insert = new SqlDataAdapter())
                        {
                            Insert.SelectCommand = new SqlCommand(insert, conn);
                            Insert.SelectCommand.Parameters.AddWithValue("@Name", Product.Name);
                            Insert.SelectCommand.Parameters.AddWithValue("@Price", Product.Price);
                            Insert.SelectCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

        }

        private static IEnumerable<Product> GetBaseData()
        {
            XmlDocument Document = new XmlDocument();
            Document.Load(BASEDATA_XML_LOCATION);

            XmlNodeList Nodes = Document.DocumentElement.SelectNodes("/products/product");
            foreach (XmlNode Node in Nodes)
            {
                yield return new Product()
                {
                    Name = Node.SelectSingleNode("name").InnerText.Trim(),
                    Price = int.Parse(Node.SelectSingleNode("price").InnerText.Trim())
                };
            }
        }

        public static Product GetProduct(int id)
        {
            using (SqlConnection conn = new SqlConnection(DB_CONN_DETAILS))
            {
                try
                {
                    conn.Open();
                    string select = "SELECT * FROM Product WHERE ID = @ID";
                    DataTable dt = new DataTable();

                    using (SqlDataAdapter GetProduct = new SqlDataAdapter())
                    {
                        GetProduct.SelectCommand = new SqlCommand(select, conn);
                        GetProduct.SelectCommand.Parameters.AddWithValue("@ID", id);
                        GetProduct.Fill(dt);
                        return MakeProducts(dt).ToList().First(); // Should be only 1 anyway
                    }
                }
                catch (Exception x)
                {
                    return null;
                }
            }
        }

        public static List<Product> GetProducts()
        {
            using (SqlConnection conn = new SqlConnection(DB_CONN_DETAILS))
            {
                try
                {
                    conn.Open();
                    string select = "SELECT * FROM Product";
                    DataTable dt = new DataTable();

                    using (SqlDataAdapter GetProducts = new SqlDataAdapter(select, conn))
                    {
                        GetProducts.Fill(dt);
                        return MakeProducts(dt).ToList();
                    }
                }
                catch (Exception x)
                {
                    // TODO: Error handling
                    throw x;
                }
            }
        }

        private static IEnumerable<Product> MakeProducts(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                yield return MakeProduct(row);
            }
        }

        private static Product MakeProduct(DataRow row)
        {
            return new Product
            {
                ID = Convert.ToInt32(row["ID"]),
                Name = row["Name"].ToString(),
                Price = Convert.ToInt32(row["Price"])
            };
        }



    }
}
