using System.Data;
using Npgsql;

namespace ShopManagementApp.Data
{
    public class PasswordResetDatabase
    {
        private readonly string connectionString =
            "Host=localhost;Port=5432;Database=shop_db;Username=postgres;Password=1234";


        // =============================================
        // ПРОВЕРКА ПОЛЬЗОВАТЕЛЯ
        // =============================================

        // =============================================
        // ПОИСК ПОЛЬЗОВАТЕЛЯ ПО ЛОГИНУ
        // =============================================

        public int? FindUserByLogin(
            string login)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT user_id
          FROM users
          WHERE login = @login";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@login",
                login);

            object? result =
                command.ExecuteScalar();

            if (result == null)
            {
                return null;
            }

            return Convert.ToInt32(result);
        }


        // =============================================
        // ПРОВЕРКА СУЩЕСТВУЮЩЕЙ ЗАЯВКИ
        // =============================================

        public bool HasActiveRequest(
            int userId)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT COUNT(*)
                  FROM password_reset_requests
                  WHERE user_id = @userId
                  AND status = 'Ожидает'";

            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@userId",
                userId);

            long count =
                Convert.ToInt64(
                    command.ExecuteScalar());


            return count > 0;
        }


        // =============================================
        // СОЗДАНИЕ ЗАЯВКИ
        // =============================================

        public void CreateRequest(
            int userId)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            using NpgsqlTransaction transaction =
                connection.BeginTransaction();


            // Блокируем аккаунт
            string blockSql =
                @"UPDATE users
                  SET is_active = FALSE
                  WHERE user_id = @userId";


            using NpgsqlCommand blockCommand =
                new NpgsqlCommand(
                    blockSql,
                    connection,
                    transaction);

            blockCommand.Parameters.AddWithValue(
                "@userId",
                userId);

            blockCommand.ExecuteNonQuery();


            // Создаём заявку
            string requestSql =
                @"INSERT INTO password_reset_requests
                  (
                      user_id,
                      status
                  )
                  VALUES
                  (
                      @userId,
                      'Ожидает'
                  )";


            using NpgsqlCommand requestCommand =
                new NpgsqlCommand(
                    requestSql,
                    connection,
                    transaction);

            requestCommand.Parameters.AddWithValue(
                "@userId",
                userId);

            requestCommand.ExecuteNonQuery();


            transaction.Commit();
        }


        // =============================================
        // ПОЛУЧЕНИЕ ЗАЯВОК ДЛЯ АДМИНИСТРАТОРА
        // =============================================

        public DataTable GetRequests()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT
                      pr.request_id,
                      pr.user_id,
                      u.login,
                      u.full_name,
                      pr.request_date,
                      pr.status
                  FROM password_reset_requests pr

                  JOIN users u
                      ON pr.user_id = u.user_id

                  WHERE pr.status = 'Ожидает'

                  ORDER BY pr.request_date DESC";


            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(command);

            DataTable table =
                new DataTable();

            adapter.Fill(table);

            return table;
        }


        // =============================================
        // КОЛИЧЕСТВО ЗАЯВОК
        // =============================================

        public int GetRequestCount()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();

            string sql =
                @"SELECT COUNT(*)
                  FROM password_reset_requests
                  WHERE status = 'Ожидает'";


            using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            return Convert.ToInt32(
                command.ExecuteScalar());
        }


        // =============================================
        // СМЕНА ПАРОЛЯ
        // =============================================

        public void CompleteRequest(
            int requestId,
            int userId,
            string newPassword)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            using NpgsqlTransaction transaction =
                connection.BeginTransaction();


            // Меняем пароль и включаем аккаунт
            string userSql =
                @"UPDATE users
                  SET
                      password = @password,
                      is_active = TRUE
                  WHERE user_id = @userId";


            using NpgsqlCommand userCommand =
                new NpgsqlCommand(
                    userSql,
                    connection,
                    transaction);

            userCommand.Parameters.AddWithValue(
                "@password",
                newPassword);

            userCommand.Parameters.AddWithValue(
                "@userId",
                userId);

            userCommand.ExecuteNonQuery();


            // Закрываем заявку
            string requestSql =
                @"UPDATE password_reset_requests
                  SET
                      status = 'Выполнена',
                      completed_date = CURRENT_TIMESTAMP
                  WHERE request_id = @requestId";


            using NpgsqlCommand requestCommand =
                new NpgsqlCommand(
                    requestSql,
                    connection,
                    transaction);

            requestCommand.Parameters.AddWithValue(
                "@requestId",
                requestId);

            requestCommand.ExecuteNonQuery();


            transaction.Commit();
        }
    }
}