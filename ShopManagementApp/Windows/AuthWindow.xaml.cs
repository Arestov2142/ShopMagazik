using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class AuthWindow : Window
    {
        private readonly Database database =
            new Database();
        // Таймер блокировки
        private readonly DispatcherTimer lockTimer =
            new DispatcherTimer();
        // Оставшееся время блокировки
        private int lockSeconds = 0;
        // Защита от повторного изменения пароля
        private bool passwordSync = false;

        public AuthWindow()
        {
            InitializeComponent();
            // Сразу устанавливаем курсор в логин
            txtLogin.Focus();
            // Таймер срабатывает каждую секунду
            lockTimer.Interval =
                TimeSpan.FromSeconds(1);
            lockTimer.Tick +=
                LockTimer_Tick;
            // Отслеживаем клавиатуру
            PreviewKeyDown +=
                AuthWindow_PreviewKeyDown;
            PreviewKeyUp +=
                AuthWindow_PreviewKeyUp;
            // Проверяем Caps Lock
            CheckCapsLock();
        }

        // =============================================
        // АВТОРИЗАЦИЯ
        // =============================================

        private void btnLogin_Click(
            object sender,
            RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            string login =
                txtLogin.Text.Trim();
            string password;
            if (chkShowPassword.IsChecked == true)
            {
                password =
                    txtVisiblePassword.Text;
            }
            else
            {
                password =
                    txtPassword.Password;
            }
            // =========================================
            // ПРОВЕРКА ЛОГИНА
            // =========================================
            if (string.IsNullOrWhiteSpace(login))
            {
                ShowError(
                    "Введите логин.");
                txtLogin.Focus();
                return;
            }
            // =========================================
            // ПРОВЕРКА ПАРОЛЯ
            // =========================================
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError(
                    "Введите пароль.");
                txtPassword.Focus();
                return;
            }
            try
            {
                txtStatus.Text =
                    "Проверка данных...";
                statusIndicator.Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            245,
                            158,
                            11));
                // Проверяем данные в PostgreSQL
                string? role =
                    database.AuthorizeUser(
                        login,
                        password);
                // =====================================
                // НЕВЕРНЫЕ ДАННЫЕ
                // =====================================
                if (role == null)
                {
                    StartLock();
                    return;
                }
                // =====================================
                // УСПЕШНЫЙ ВХОД
                // =====================================
                HideError();
                txtStatus.Text =
                    "Авторизация выполнена";
                statusIndicator.Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            22,
                            163,
                            74));
                // =====================================
                // МЕНЕДЖЕР
                // =====================================
                if (role == "Менеджер")
                {
                    MainWindow mainWindow =
                        new MainWindow();
                    mainWindow.Show();
                    Close();
                    return;
                }
                // =====================================
                // АДМИНИСТРАТОР
                // =====================================
                if (role == "Администратор")
                {
                    AdminWindow adminWindow =
                        new AdminWindow();
                    adminWindow.Show();
                    Close();
                    return;
                }
                ShowError(
                    "Для пользователя не определена роль.");
            }
            catch (Exception ex)
            {
                ShowError(
                    "Ошибка подключения к базе данных.");
                txtStatus.Text =
                    "Ошибка PostgreSQL";
                statusIndicator.Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            220,
                            38,
                            38));
                MessageBox.Show(
                    ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // БЛОКИРОВКА НА 3 СЕКУНДЫ
        // =============================================
        private void StartLock()
        {
            lockSeconds = 3;
            // Блокируем элементы
            txtLogin.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtVisiblePassword.IsEnabled = false;
            chkShowPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnClear.IsEnabled = false;
            ShowError(
                "Неверный логин или пароль.");
            txtStatus.Text =
                "Повторный вход через 3 сек.";
            statusIndicator.Fill =
                new SolidColorBrush(
                    Color.FromRgb(
                        220,
                        38,
                        38));
            lockTimer.Start();
        }
        // =============================================
        // ТАЙМЕР
        // =============================================
        private void LockTimer_Tick(
            object? sender,
            EventArgs e)
        {
            lockSeconds--;
            if (lockSeconds > 0)
            {
                txtStatus.Text =
                    "Повторный вход через " +
                    lockSeconds +
                    " сек.";
                return;
            }
            lockTimer.Stop();
            // Разблокируем элементы
            txtLogin.IsEnabled = true;
            txtPassword.IsEnabled = true;
            txtVisiblePassword.IsEnabled = true;
            chkShowPassword.IsEnabled = true;
            btnLogin.IsEnabled = true;
            btnClear.IsEnabled = true;
            txtStatus.Text =
                "Можно повторить авторизацию";
            statusIndicator.Fill =
                new SolidColorBrush(
                    Color.FromRgb(
                        22,
                        163,
                        74));
            // Очищаем только пароль
            txtPassword.Clear();
            txtVisiblePassword.Clear();
            HideError();
            if (chkShowPassword.IsChecked == true)
            {
                txtVisiblePassword.Focus();
            }
            else
            {
                txtPassword.Focus();
            }
        }
        // =============================================
        // ПОКАЗАТЬ ПАРОЛЬ
        // =============================================
        private void chkShowPassword_Checked(
            object sender,
            RoutedEventArgs e)
        {
            passwordSync = true;
            txtVisiblePassword.Text =
                txtPassword.Password;
            txtPassword.Visibility =
                Visibility.Collapsed;
            txtVisiblePassword.Visibility =
                Visibility.Visible;
            txtVisiblePassword.Focus();
            txtVisiblePassword.CaretIndex =
                txtVisiblePassword.Text.Length;
            passwordSync = false;
        }

        // =============================================
        // СКРЫТЬ ПАРОЛЬ
        // =============================================
        private void chkShowPassword_Unchecked(
            object sender,
            RoutedEventArgs e)
        {
            passwordSync = true;
            txtPassword.Password =
                txtVisiblePassword.Text;
            txtVisiblePassword.Visibility =
                Visibility.Collapsed;
            txtPassword.Visibility =
                Visibility.Visible;
            txtPassword.Focus();
            passwordSync = false;
        }

        // =============================================
        // СИНХРОНИЗАЦИЯ СКРЫТОГО ПАРОЛЯ
        // =============================================

        private void txtPassword_PasswordChanged(
            object sender,
            RoutedEventArgs e)
        {
            if (passwordSync)
            {
                return;
            }
            passwordSync = true;
            txtVisiblePassword.Text =
                txtPassword.Password;
            passwordSync = false;
            HideError();
        }

        // =============================================
        // СИНХРОНИЗАЦИЯ ОТКРЫТОГО ПАРОЛЯ
        // =============================================
        private void txtVisiblePassword_TextChanged(
            object sender,
            System.Windows.Controls.TextChangedEventArgs e)
        {
            if (passwordSync)
            {
                return;
            }
            passwordSync = true;
            txtPassword.Password =
                txtVisiblePassword.Text;
            passwordSync = false;
            HideError();
        }


        // =============================================
        // ИЗМЕНЕНИЕ ЛОГИНА
        // =============================================
        private void txtLogin_TextChanged(
            object sender,
            System.Windows.Controls.TextChangedEventArgs e)
        {
            HideError();
        }


        // =============================================
        // ОЧИСТИТЬ ПОЛЯ
        // =============================================
        private void btnClear_Click(
            object sender,
            RoutedEventArgs e)
        {
            txtLogin.Clear();
            txtPassword.Clear();
            txtVisiblePassword.Clear();
            chkShowPassword.IsChecked =
                false;
            HideError();
            txtStatus.Text =
                "Поля очищены";
            statusIndicator.Fill =
                new SolidColorBrush(
                    Color.FromRgb(
                        79,
                        70,
                        229));
            txtLogin.Focus();
        }


        // =============================================
        // ENTER В PASSWORDBOX
        // =============================================
        private void txtPassword_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                btnLogin.IsEnabled)
            {
                Login();
            }
        }


        // =============================================
        // ENTER В ОТКРЫТОМ ПАРОЛЕ
        // =============================================

        private void txtVisiblePassword_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                btnLogin.IsEnabled)
            {
                Login();
            }
        }


        // =============================================
        // CAPS LOCK
        // =============================================
        private void AuthWindow_PreviewKeyDown(
            object sender,
            KeyEventArgs e)
        {
            CheckCapsLock();
        }
        private void AuthWindow_PreviewKeyUp(
            object sender,
            KeyEventArgs e)
        {
            CheckCapsLock();
        }
        private void CheckCapsLock()
        {
            bool capsLock =
                Keyboard.IsKeyToggled(
                    Key.CapsLock);
            if (capsLock)
            {
                txtCapsLock.Visibility =
                    Visibility.Visible;
            }
            else
            {
                txtCapsLock.Visibility =
                    Visibility.Collapsed;
            }
        }


        // =============================================
        // ПОКАЗАТЬ ОШИБКУ
        // =============================================
        private void ShowError(
            string message)
        {
            txtError.Text =
                message;
            statusBorder.Visibility =
                Visibility.Visible;
        }


        // =============================================
        // СКРЫТЬ ОШИБКУ
        // =============================================
        private void HideError()
        {
            txtError.Text =
                "";
            statusBorder.Visibility =
                Visibility.Collapsed;
        }
        // =============================================
        // ЗАБЫЛИ ПАРОЛЬ
        // =============================================
        private void btnForgotPassword_Click(
            object sender,
            RoutedEventArgs e)
        {
            ForgotPasswordWindow forgotWindow =
                new ForgotPasswordWindow();
            forgotWindow.Owner =
                this;
            forgotWindow.ShowDialog();
        }
    }
}