using System;
using System.Windows;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly PasswordResetDatabase passwordResetDatabase =
            new PasswordResetDatabase();
        public ForgotPasswordWindow()
        {
            InitializeComponent();
            txtLogin.Focus();
        }
        // =============================================
        // ОТПРАВКА ЗАЯВКИ
        // =============================================
        private void btnSendRequest_Click(
            object sender,
            RoutedEventArgs e)
        {
            string login =
                txtLogin.Text.Trim();
            // Проверяем заполнение логина
            if (string.IsNullOrWhiteSpace(login))
            {
                txtError.Text =
                    "Введите логин.";
                txtLogin.Focus();
                return;
            }
            try
            {
                // Ищем пользователя по логину
                int? userId =
                    passwordResetDatabase
                        .FindUserByLogin(login);
                // Пользователь не найден
                if (userId == null)
                {
                    txtError.Text =
                        "Пользователь с таким логином не найден.";
                    return;
                }
                // Проверяем,
                // нет ли уже активной заявки
                if (passwordResetDatabase.HasActiveRequest(
                    userId.Value))
                {
                    txtError.Text =
                        "Заявка на смену пароля уже отправлена.";
                    return;
                }
                // Подтверждение
                MessageBoxResult result =
                    MessageBox.Show(
                        "После отправки заявки аккаунт будет временно заблокирован.\n\n" +
                        "Продолжить?",
                        "Восстановление пароля",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
                // Создаём заявку
                passwordResetDatabase.CreateRequest(
                    userId.Value);
                MessageBox.Show(
                    "Заявка отправлена администратору.\n\n" +
                    "Аккаунт временно заблокирован до установки нового пароля.",
                    "Заявка создана",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // =============================================
        // ОТМЕНА
        // =============================================
        private void btnCancel_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}