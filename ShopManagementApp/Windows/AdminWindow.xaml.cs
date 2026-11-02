using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class AdminWindow : Window
    {
        // =============================================
        // БАЗА ПОЛЬЗОВАТЕЛЕЙ
        // =============================================
        private readonly UserDatabase userDatabase =
            new UserDatabase();
        // =============================================
        // БАЗА ЗАЯВОК НА ВОССТАНОВЛЕНИЕ ПАРОЛЯ
        // =============================================
        private readonly PasswordResetDatabase passwordResetDatabase =
            new PasswordResetDatabase();
        // =============================================
        // ФЛАГ ЗАГРУЗКИ ПОЛЬЗОВАТЕЛЯ
        // =============================================
        // Нужен для того, чтобы при выборе пользователя
        // из таблицы логин и почта не генерировались заново.
        private bool isLoadingUser = false;
        // =============================================
        // КОНСТРУКТОР
        // =============================================
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles();
            LoadPasswordRequestCount();
            txtFullName.Focus();
        }
        // =============================================
        // ЗАГРУЗКА ПОЛЬЗОВАТЕЛЕЙ
        // =============================================
        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource =
                    userDatabase
                        .GetUsers()
                        .DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки пользователей:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ЗАГРУЗКА РОЛЕЙ
        // =============================================
        private void LoadRoles()
        {
            try
            {
                cmbRole.ItemsSource =
                    userDatabase
                        .GetRoles()
                        .DefaultView;
                cmbRole.DisplayMemberPath =
                    "role_name";
                cmbRole.SelectedValuePath =
                    "role_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки ролей:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // КОЛИЧЕСТВО ЗАЯВОК НА ПАРОЛЬ
        // =============================================
        private void LoadPasswordRequestCount()
        {
            try
            {
                int count =
                    passwordResetDatabase
                        .GetRequestCount();
                txtRequestCount.Text =
                    count.ToString();
                if (count > 0)
                {
                    requestCountBorder.Visibility =
                        Visibility.Visible;
                }
                else
                {
                    requestCountBorder.Visibility =
                        Visibility.Collapsed;
                }
            }
            catch
            {
                txtRequestCount.Text =
                    "0";
                requestCountBorder.Visibility =
                    Visibility.Collapsed;
            }
        }
        // =============================================
        // ИЗМЕНЕНИЕ ФИО
        // =============================================
        private void txtFullName_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            // Если сейчас загружается пользователь
            // из таблицы, ничего не генерируем.
            if (isLoadingUser)
            {
                return;
            }
            // Если пользователь выбран в таблице,
            // значит сейчас режим редактирования.
            // Старый логин и почту сохраняем.
            if (dgUsers != null &&
                dgUsers.SelectedItem != null)
            {
                return;
            }
            string fullName =
                txtFullName.Text.Trim();
            // Если ФИО пустое
            if (string.IsNullOrWhiteSpace(fullName))
            {
                txtLogin.Clear();
                txtEmail.Clear();
                return;
            }
            try
            {
                string login =
                    GenerateLogin(fullName);
                txtLogin.Text =
                    login;
                if (!string.IsNullOrWhiteSpace(login))
                {
                    txtEmail.Text =
                        login +
                        "@ShopMagazik.ru";
                }
                else
                {
                    txtEmail.Clear();
                }
            }
            catch
            {
                txtLogin.Clear();
                txtEmail.Clear();
            }
        }
        // =============================================
        // ГЕНЕРАЦИЯ ЛОГИНА
        // =============================================
        private string GenerateLogin(
            string fullName)
        {
            string[] parts =
                fullName.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);
            // Нет ни одного слова
            if (parts.Length == 0)
            {
                return "";
            }
            // =========================================
            // ФАМИЛИЯ
            // =========================================
            string lastName =
                Transliterate(
                    parts[0]);
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return "";
            }
            // =========================================
            // ИНИЦИАЛЫ
            // =========================================
            string initials =
                "";
            // Имя
            if (parts.Length >= 2)
            {
                string firstName =
                    Transliterate(
                        parts[1]);
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    initials +=
                        firstName[0];
                }
            }
            // Отчество
            if (parts.Length >= 3)
            {
                string middleName =
                    Transliterate(
                        parts[2]);
                if (!string.IsNullOrWhiteSpace(middleName))
                {
                    initials +=
                        middleName[0];
                }
            }


            // =========================================
            // ОСНОВА ЛОГИНА
            // =========================================
            string baseLogin =
                lastName.ToLower();
            if (!string.IsNullOrWhiteSpace(initials))
            {
                baseLogin +=
                    "." +
                    initials.ToLower();
            }
            // Удаляем лишние символы
            baseLogin =
                CleanLogin(baseLogin);
            string login =
                baseLogin;
            int number =
                2;
            // =========================================
            // ПРОВЕРКА УНИКАЛЬНОСТИ
            // =========================================
            while (userDatabase.LoginExists(login))
            {
                login =
                    baseLogin +
                    number;
                number++;
            }
            return login;
        }
        // =============================================
        // ОЧИСТКА ЛОГИНА
        // =============================================
        private string CleanLogin(
            string login)
        {
            StringBuilder result =
                new StringBuilder();
            foreach (char symbol in login)
            {
                // Разрешаем:
                // английские буквы
                // цифры
                // точку
                if (
                    (symbol >= 'a' && symbol <= 'z') ||
                    (symbol >= 'A' && symbol <= 'Z') ||
                    (symbol >= '0' && symbol <= '9') ||
                    symbol == '.')
                {
                    result.Append(symbol);
                }
            }
            return result
                .ToString()
                .ToLower();
        }
        // =============================================
        // ТРАНСЛИТЕРАЦИЯ
        // =============================================
        private string Transliterate(
            string text)
        {
            string result =
                text.ToLower();
            result = result
                .Replace("а", "a")
                .Replace("б", "b")
                .Replace("в", "v")
                .Replace("г", "g")
                .Replace("д", "d")
                .Replace("е", "e")
                .Replace("ё", "e")
                .Replace("ж", "zh")
                .Replace("з", "z")
                .Replace("и", "i")
                .Replace("й", "y")
                .Replace("к", "k")
                .Replace("л", "l")
                .Replace("м", "m")
                .Replace("н", "n")
                .Replace("о", "o")
                .Replace("п", "p")
                .Replace("р", "r")
                .Replace("с", "s")
                .Replace("т", "t")
                .Replace("у", "u")
                .Replace("ф", "f")
                .Replace("х", "h")
                .Replace("ц", "ts")
                .Replace("ч", "ch")
                .Replace("ш", "sh")
                .Replace("щ", "sch")
                .Replace("ъ", "")
                .Replace("ы", "y")
                .Replace("ь", "")
                .Replace("э", "e")
                .Replace("ю", "yu")
                .Replace("я", "ya");
            return result;
        }
        // =============================================
        // ДОБАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯ
        // =============================================
        private void btnAddUser_Click(
            object sender,
            RoutedEventArgs e)
        {
            // =========================================
            // ПРОВЕРКА ФИО
            // =========================================
            if (string.IsNullOrWhiteSpace(
                txtFullName.Text))
            {
                MessageBox.Show(
                    "Введите ФИО пользователя.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtFullName.Focus();
                return;
            }
            // =========================================
            // ПРОВЕРКА ЛОГИНА
            // =========================================
            if (string.IsNullOrWhiteSpace(
                txtLogin.Text))
            {
                MessageBox.Show(
                    "Не удалось сформировать логин.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            // =========================================
            // ПРОВЕРКА ПОЧТЫ
            // =========================================
            if (string.IsNullOrWhiteSpace(
                txtEmail.Text))
            {
                MessageBox.Show(
                    "Не удалось сформировать корпоративную почту.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            // =========================================
            // ПРОВЕРКА РОЛИ
            // =========================================
            if (cmbRole.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите роль пользователя.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cmbRole.Focus();
                return;
            }
            // =========================================
            // ПРОВЕРКА ПАРОЛЯ
            // =========================================
            if (string.IsNullOrWhiteSpace(
                txtPassword.Password))
            {
                MessageBox.Show(
                    "Введите пароль пользователя.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }
            try
            {
                int roleId =
                    Convert.ToInt32(
                        cmbRole.SelectedValue);
                bool isActive =
                    chkActive.IsChecked == true;
                // =====================================
                // ДОБАВЛЯЕМ ПОЛЬЗОВАТЕЛЯ
                // =====================================
                userDatabase.AddUser(
                    txtLogin.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtPassword.Password,
                    txtFullName.Text.Trim(),
                    roleId,
                    isActive);
                MessageBox.Show(
                    "Пользователь успешно создан.\n\n" +
                    "Логин: " +
                    txtLogin.Text +
                    "\n" +
                    "Почта: " +
                    txtEmail.Text,
                    "Пользователь создан",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ClearFields();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка создания пользователя:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ИЗМЕНЕНИЕ ПОЛЬЗОВАТЕЛЯ
        // =============================================
        private void btnEditUser_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите пользователя в таблице.",
                    "Редактирование",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(
                txtFullName.Text))
            {
                MessageBox.Show(
                    "Введите ФИО пользователя.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtFullName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(
                txtLogin.Text))
            {
                MessageBox.Show(
                    "Логин пользователя отсутствует.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(
                txtEmail.Text))
            {
                MessageBox.Show(
                    "Корпоративная почта отсутствует.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            if (cmbRole.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите роль пользователя.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            DataRowView row =
                (DataRowView)dgUsers.SelectedItem;
            int userId =
                Convert.ToInt32(
                    row["user_id"]);
            int roleId =
                Convert.ToInt32(
                    cmbRole.SelectedValue);
            bool isActive =
                chkActive.IsChecked == true;
            try
            {
                userDatabase.UpdateUser(
                    userId,
                    txtLogin.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtFullName.Text.Trim(),
                    roleId,
                    isActive);
                MessageBox.Show(
                    "Данные пользователя успешно изменены.",
                    "Готово",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ClearFields();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка изменения пользователя:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ИЗМЕНЕНИЕ ПАРОЛЯ
        // =============================================
        private void btnChangePassword_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите пользователя в таблице.",
                    "Изменение пароля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(
                txtPassword.Password))
            {
                MessageBox.Show(
                    "Введите новый пароль.",
                    "Изменение пароля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }
            DataRowView row =
                (DataRowView)dgUsers.SelectedItem;
            int userId =
                Convert.ToInt32(
                    row["user_id"]);
            string login =
                row["login"].ToString() ?? "";
            MessageBoxResult result =
                MessageBox.Show(
                    "Изменить пароль пользователя \"" +
                    login +
                    "\"?",
                    "Изменение пароля",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                userDatabase.UpdateUserPassword(
                    userId,
                    txtPassword.Password);
                MessageBox.Show(
                    "Пароль пользователя успешно изменён.",
                    "Готово",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка изменения пароля:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // УДАЛЕНИЕ ПОЛЬЗОВАТЕЛЯ
        // =============================================
        private void btnDeleteUser_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите пользователя в таблице.",
                    "Удаление",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            DataRowView row =
                (DataRowView)dgUsers.SelectedItem;
            int userId =
                Convert.ToInt32(
                    row["user_id"]);
            string login =
                row["login"].ToString() ?? "";
            string fullName =
                row["full_name"].ToString() ?? "";
            MessageBoxResult result =
                MessageBox.Show(
                    "Вы действительно хотите удалить пользователя?\n\n" +
                    "ФИО: " +
                    fullName +
                    "\n" +
                    "Логин: " +
                    login,
                    "Удаление пользователя",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                userDatabase.DeleteUser(
                    userId);
                MessageBox.Show(
                    "Пользователь удалён.",
                    "Готово",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ClearFields();
                LoadUsers();
                LoadPasswordRequestCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка удаления пользователя:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ВЫБОР ПОЛЬЗОВАТЕЛЯ В ТАБЛИЦЕ
        // =============================================
        private void dgUsers_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                return;
            }
            if (dgUsers.SelectedItem is not DataRowView row)
            {
                return;
            }
            // Отключаем автоматическую генерацию,
            // пока заполняем поля.
            isLoadingUser =
                true;
            try
            {
                // =====================================
                // ФИО
                // =====================================
                txtFullName.Text =
                    row["full_name"].ToString();
                // =====================================
                // ЛОГИН
                // =====================================
                txtLogin.Text =
                    row["login"].ToString();
                // =====================================
                // ПОЧТА
                // =====================================
                txtEmail.Text =
                    row["email"].ToString();
                // =====================================
                // РОЛЬ
                // =====================================
                cmbRole.SelectedValue =
                    Convert.ToInt32(
                        row["role_id"]);
                // =====================================
                // СОСТОЯНИЕ
                // =====================================
                chkActive.IsChecked =
                    Convert.ToBoolean(
                        row["is_active"]);
                // =====================================
                // ПАРОЛЬ
                // =====================================
                // Старый пароль в поле
                // никогда не выводим.
                txtPassword.Clear();
            }
            finally
            {
                isLoadingUser =
                    false;
            }
        }
        // =============================================
        // ОБНОВЛЕНИЕ
        // =============================================
        private void btnRefresh_Click(
            object sender,
            RoutedEventArgs e)
        {
            txtSearch.Clear();
            ClearFields();
            LoadUsers();
            LoadRoles();
            LoadPasswordRequestCount();
        }
        // =============================================
        // ОЧИСТКА ПОЛЕЙ
        // =============================================
        private void ClearFields()
        {
            isLoadingUser =
                true;
            try
            {
                dgUsers.SelectedItem =
                    null;
                txtFullName.Clear();
                txtLogin.Clear();
                txtEmail.Clear();
                txtPassword.Clear();
                cmbRole.SelectedIndex =
                    -1;
                chkActive.IsChecked =
                    true;
            }
            finally
            {
                isLoadingUser =
                    false;
            }
            txtFullName.Focus();
        }
        // =============================================
        // ПОИСК
        // =============================================
        private void btnSearch_Click(
            object sender,
            RoutedEventArgs e)
        {
            SearchUsers();
        }
        // =============================================
        // ПОИСК ПОЛЬЗОВАТЕЛЕЙ
        // =============================================
        private void SearchUsers()
        {
            string lastName =
                txtSearch.Text.Trim();
            // Если поле пустое —
            // показываем всех пользователей.
            if (string.IsNullOrWhiteSpace(lastName))
            {
                LoadUsers();
                return;
            }
            try
            {
                dgUsers.ItemsSource =
                    userDatabase
                        .SearchUsersByLastName(
                            lastName)
                        .DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка поиска:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ПОИСК ПО ENTER
        // =============================================
        private void txtSearch_KeyDown(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key ==
                System.Windows.Input.Key.Enter)
            {
                SearchUsers();
            }
        }
        // =============================================
        // ОКНО ЗАЯВОК НА СМЕНУ ПАРОЛЯ
        // =============================================
        private void btnPasswordRequests_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                PasswordRequestsWindow window =
                    new PasswordRequestsWindow();
                window.Owner =
                    this;
                window.ShowDialog();
                // После закрытия окна обновляем
                // количество заявок.
                LoadPasswordRequestCount();
                // Аккаунт пользователя мог стать
                // снова активным.
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось открыть заявки:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ВЫХОД ИЗ АККАУНТА
        // =============================================
        private void btnLogout_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Вы действительно хотите выйти из аккаунта?",
                    "Выход",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            AuthWindow authWindow =
                new AuthWindow();
            authWindow.Show();
            Close();
        }
    }
}