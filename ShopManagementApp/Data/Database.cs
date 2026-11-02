using System.Data;
using Npgsql;

namespace ShopManagementApp.Data
{
    public class Database
    {
        private readonly string connectionString =
            "Host=localhost;Port=5432;Database=shop_db1;Username=postgres;Password=1234";

        public DataTable GetClients()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT
                    client_id,
                    full_name,
                    phone,
                    email,
                    address
                  FROM clients
                  ORDER BY client_id";

            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(sql, connection);

            DataTable table = new DataTable();

            adapter.Fill(table);

            return table;
        }


        // Добавление клиента
        public void AddClient(
            string fullName,
            string phone,
            string email,
            string address)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"INSERT INTO clients
                  (full_name, phone, email, address)
                  VALUES
                  (@name, @phone, @email, @address)";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", fullName);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@address", address);

            command.ExecuteNonQuery();
        }


        // Изменение клиента
        public void UpdateClient(
            int clientId,
            string fullName,
            string phone,
            string email,
            string address)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"UPDATE clients
                  SET
                    full_name = @name,
                    phone = @phone,
                    email = @email,
                    address = @address
                  WHERE client_id = @id";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", fullName);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@address", address);
            command.Parameters.AddWithValue("@id", clientId);

            command.ExecuteNonQuery();
        }


        // Удаление клиента
        public void DeleteClient(int clientId)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"DELETE FROM clients
                  WHERE client_id = @id";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", clientId);

            command.ExecuteNonQuery();
        }


        // Получение списка товаров
        public DataTable GetProducts()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT
                    product_id,
                    product_name,
                    price,
                    quantity_in_stock
                  FROM products
                  ORDER BY product_id";

            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(sql, connection);

            DataTable table = new DataTable();

            adapter.Fill(table);

            return table;
        }


        // Добавление товара
        public void AddProduct(
            string productName,
            decimal price,
            int quantity)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"INSERT INTO products
                  (product_name, price, quantity_in_stock)
                  VALUES
                  (@name, @price, @quantity)";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", productName);
            command.Parameters.AddWithValue("@price", price);
            command.Parameters.AddWithValue("@quantity", quantity);

            command.ExecuteNonQuery();
        }


        // Изменение товара
        public void UpdateProduct(
            int productId,
            string productName,
            decimal price,
            int quantity)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"UPDATE products
                  SET
                    product_name = @name,
                    price = @price,
                    quantity_in_stock = @quantity
                  WHERE product_id = @id";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", productName);
            command.Parameters.AddWithValue("@price", price);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@id", productId);

            command.ExecuteNonQuery();
        }


        // Удаление товара
        public void DeleteProduct(int productId)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"DELETE FROM products
                  WHERE product_id = @id";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", productId);

            command.ExecuteNonQuery();
        }


        // =====================================================
        // ЗАКАЗЫ
        // =====================================================

        // Получение списка заказов
        public DataTable GetOrders()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT
                    o.order_id,
                    o.client_id,
                    o.product_id,
                    c.full_name AS client,
                    p.product_name AS product,
                    o.quantity,
                    p.price,
                    o.quantity * p.price AS total_price,
                    o.order_date,
                    o.status
                  FROM orders o

                  JOIN clients c
                    ON o.client_id = c.client_id

                  JOIN products p
                    ON o.product_id = p.product_id

                  ORDER BY o.order_id";

            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(sql, connection);

            DataTable table = new DataTable();

            adapter.Fill(table);

            return table;
        }


        // Добавление заказа
        public void AddOrder(
            int clientId,
            int productId,
            int quantity,
            string status)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"INSERT INTO orders
                  (client_id, product_id, quantity, status)
                  VALUES
                  (@client, @product, @quantity, @status)";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@client", clientId);
            command.Parameters.AddWithValue("@product", productId);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@status", status);

            command.ExecuteNonQuery();
        }


        // Изменение заказа
        public void UpdateOrder(
            int orderId,
            int clientId,
            int productId,
            int quantity,
            string status)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"UPDATE orders
                  SET
                    client_id = @client,
                    product_id = @product,
                    quantity = @quantity,
                    status = @status
                  WHERE order_id = @id";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@client", clientId);
            command.Parameters.AddWithValue("@product", productId);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", orderId);

            command.ExecuteNonQuery();
        }


        // Удаление заказа
        public void DeleteOrder(int orderId)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"DELETE FROM orders
                  WHERE order_id = @id";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", orderId);

            command.ExecuteNonQuery();
        }


        // =====================================================
        // COMBOBOX
        // =====================================================

        // Получение клиентов для ComboBox
        public DataTable GetClientsForComboBox()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT
                    client_id,
                    full_name
                  FROM clients
                  ORDER BY full_name";

            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(sql, connection);

            DataTable table = new DataTable();

            adapter.Fill(table);

            return table;
        }


        // Получение товаров для ComboBox
        public DataTable GetProductsForComboBox()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT
                    product_id,
                    product_name
                  FROM products
                  ORDER BY product_name";

            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(sql, connection);

            DataTable table = new DataTable();

            adapter.Fill(table);

            return table;
        }
        // Авторизация пользователя
        public string? AuthorizeUser(
            string login,
            string password)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT r.role_name
          FROM users u

          JOIN roles r
              ON u.role_id = r.role_id

          WHERE u.login = @login
          AND u.password = @password
          AND u.is_active = TRUE";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@login",
                login);

            command.Parameters.AddWithValue(
                "@password",
                password);

            object? result =
                command.ExecuteScalar();

            if (result == null)
            {
                return null;
            }

            return result.ToString();
        }
    }
}