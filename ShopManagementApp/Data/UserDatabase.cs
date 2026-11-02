using System;
using System.Data;
using Npgsql;

namespace ShopManagementApp.Data
{
    public class UserDatabase
    {
        private readonly string connectionString =
            "Host=localhost;Port=5432;Database=shop_db;Username=postgres;Password=1234";


        // =============================================
        // ПОЛУЧЕНИЕ ВСЕХ ПОЛЬЗОВАТЕЛЕЙ
        // =============================================

        public DataTable GetUsers()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            string sql =
                @"SELECT
                      u.user_id,
                      u.login,
                      u.full_name,
                      u.email,
                      u.role_id,
                      r.role_name,
                      u.is_active,
                      u.created_at
                  FROM users u

                  JOIN roles r
                      ON u.role_id = r.role_id

                  ORDER BY u.user_id";


            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);


            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(
                    command);


            DataTable table =
                new DataTable();


            adapter.Fill(
                table);


            return table;
        }


        // =============================================
        // ПОИСК ПОЛЬЗОВАТЕЛЯ ПО ФАМИЛИИ
        // =============================================

        public DataTable SearchUsersByLastName(
            string lastName)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            string sql =
                @"SELECT
                      u.user_id,
                      u.login,
                      u.full_name,
                      u.email,
                      u.role_id,
                      r.role_name,
                      u.is_active,
                      u.created_at
                  FROM users u

                  JOIN roles r
                      ON u.role_id = r.role_id

                  WHERE u.full_name
                        ILIKE @lastName

                  ORDER BY u.full_name";


            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);


            command.Parameters.AddWithValue(
                "@lastName",
                lastName.Trim() + "%");


            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(
                    command);


            DataTable table =
                new DataTable();


            adapter.Fill(
                table);


            return table;
        }


        // =============================================
        // ПОЛУЧЕНИЕ РОЛЕЙ
        // =============================================

        public DataTable GetRoles()
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            string sql =
                @"SELECT
                      role_id,
                      role_name
                  FROM roles
                  ORDER BY role_id";


            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);


            using NpgsqlDataAdapter adapter =
                new NpgsqlDataAdapter(
                    command);


            DataTable table =
                new DataTable();


            adapter.Fill(
                table);


            return table;
        }


        // =============================================
        // ПРОВЕРКА СУЩЕСТВОВАНИЯ ЛОГИНА
        // =============================================

        public bool LoginExists(
            string login)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            string sql =
                @"SELECT COUNT(*)
                  FROM users
                  WHERE LOWER(login) =
                        LOWER(@login)";


            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);


            command.Parameters.AddWithValue(
                "@login",
                login);


            long count =
                Convert.ToInt64(
                    command.ExecuteScalar());


            return count > 0;
        }


        // =============================================
        // ДОБАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯ
        // =============================================

        public void AddUser(
            string login,
            string email,
            string password,
            string fullName,
            int roleId,
            bool isActive)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);

            connection.Open();


            string sql =
                @"INSERT INTO users
                  (
                      login,
                      email,
                      password,
                      full_name,
                      role_id,
                      is_active
                  )
                  VALUES
                  (
                      @login,
                      @email,
                      @password,
                      @fullName,
                      @roleId,
                      @isActive
                  )";


            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);


            command.Parameters.AddWithValue(
                "@login",
                login);


            command.Parameters.AddWithValue(
                "@email",
                email);


            command.Parameters.AddWithValue(
                "@password",
                password);


            command.Parameters.AddWithValue(
                "@fullName",
                fullName);


            command.Parameters.AddWithValue(
                "@roleId",
                roleId);


            command.Parameters.AddWithValue(
                "@isActive",
                isActive);


            command.ExecuteNonQuery();
        }


        // =============================================
        // ИЗМЕНЕНИЕ ПОЛЬЗОВАТЕЛЯ
        // =============================================

        public void UpdateUser(
            int userId,
            string login,
            string email,
            string fullName,
            int roleId,
            bool isActive)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);
            connection.Open();
            string sql =
                @"UPDATE users
                  SET
                      login = @login,
                      email = @email,
                      full_name = @fullName,
                      role_id = @roleId,
                      is_active = @isActive
                  WHERE user_id = @userId";
            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);
            command.Parameters.AddWithValue(
                "@login",
                login);
            command.Parameters.AddWithValue(
                "@email",
                email);
            command.Parameters.AddWithValue(
                "@fullName",
                fullName);
            command.Parameters.AddWithValue(
                "@roleId",
                roleId);
            command.Parameters.AddWithValue(
                "@isActive",
                isActive);
            command.Parameters.AddWithValue(
                "@userId",
                userId);
            command.ExecuteNonQuery();
        }


        // =============================================
        // ИЗМЕНЕНИЕ ПАРОЛЯ
        // =============================================

        public void UpdateUserPassword(
            int userId,
            string password)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);
            connection.Open();
            string sql =
                @"UPDATE users
                  SET password = @password
                  WHERE user_id = @userId";
            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);
            command.Parameters.AddWithValue(
                "@password",
                password);
            command.Parameters.AddWithValue(
                "@userId",
                userId);
            command.ExecuteNonQuery();
        }

        // =============================================
        // УДАЛЕНИЕ ПОЛЬЗОВАТЕЛЯ
        // =============================================
        public void DeleteUser(
            int userId)
        {
            using NpgsqlConnection connection =
                new NpgsqlConnection(connectionString);
            connection.Open();
            string sql =
                @"DELETE FROM users
                  WHERE user_id = @userId";
            using NpgsqlCommand command =
                new NpgsqlCommand(
                    sql,
                    connection);
            command.Parameters.AddWithValue(
                "@userId",
                userId);

            command.ExecuteNonQuery();
        }
    }
}