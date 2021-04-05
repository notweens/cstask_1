using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=db;Integrated Security=True;";

        static List<Product> products = new List<Product>();
        static List<Order> orders = new List<Order>();

        static async Task<bool> LoadData()
        {
            bool state = false;
            await Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        state = true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Connection is not established.");
                        Console.ReadLine();
                    }

                    using (SqlCommand command = new SqlCommand("SELECT * FROM products", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product((uint)reader.GetInt32(0), reader.GetString(1), (uint)reader.GetInt32(2)));
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand("SELECT * FROM orderspos LEFT JOIN orders ON orderspos.orderId = orders.id", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool exists = false;
                                foreach (var order in orders)
                                {
                                    if (order.Id == (uint)reader.GetInt32(4))
                                    {
                                        order.Items.Add(products[products.FindIndex(i => i.Id == (uint)reader.GetInt32(2))], (uint)reader.GetInt32(3));
                                        exists = true;
                                    }
                                }
                                if (!exists)
                                {
                                    Dictionary<Product, uint> items = new Dictionary<Product, uint>
                                    {
                                        { products[products.FindIndex(i => i.Id == (uint)reader.GetInt32(2))], (uint)reader.GetInt32(3) }
                                    };
                                    orders.Add(new Order((uint)reader.GetInt32(4), (uint)reader.GetInt32(5), reader.GetDateTime(6), items));
                                }
                            }
                        }
                    }

                    connection.Close();
                }
            });
            return state;
        }

        static void UserInput()
        {
            string clientName = "";
            int clientId = -1;

            while (true)
            {
                Console.WriteLine("Введите имя клиента и нажмите Enter:");

                clientId = -1;
                clientName = "";

                while (clientName.Equals(""))
                {
                    clientName = Console.ReadLine();
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Connection is not established.");
                        Console.ReadLine();
                        break;
                    }

                    using (SqlCommand command = new SqlCommand("SELECT TOP 1 id FROM clients WHERE name = @name", connection))
                    {
                        command.Parameters.AddWithValue("name", clientName);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    clientId = reader.GetInt32(0);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Нет данных");
                            }
                        }
                    }

                    List<Order> clientOrders = orders.FindAll(i => i.Buyer == clientId);
                    uint totalPrice = 0, counter = 0;

                    Console.WriteLine("Найдено заказов: {0}", clientOrders.Count);

                    foreach (var order in clientOrders)
                    {
                        Console.WriteLine("-----");
                        Console.WriteLine("Заказ №{0}, Дата создания {1}", order.Id, order.Date.ToShortDateString());
                        Console.WriteLine("Состав:");
                        
                        foreach (KeyValuePair<Product, uint> item in order.Items)
                        {
                            totalPrice = item.Value * item.Key.Price;
                            
                            Console.WriteLine("{0} - {1} руб. ({2} шт.) - {3} руб.", item.Key.Name, item.Key.Price, item.Value, totalPrice);

                            counter += totalPrice;
                        }

                        Console.WriteLine("Общая стоимость: {0} руб.", counter);
                        Console.WriteLine("-----");
                    }

                    connection.Close();
                }
            }
        }

        static void Main(string[] args)
        {
            if (!LoadData().Result)
            {
                return;
            }

            UserInput();
        }
    }
}
