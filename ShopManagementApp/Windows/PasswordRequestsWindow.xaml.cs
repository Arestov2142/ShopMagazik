using System;
using System.Data;
using System.Windows;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class PasswordRequestsWindow : Window
    {
        private readonly PasswordResetDatabase passwordResetDatabase =
            new PasswordResetDatabase();
        public PasswordRequestsWindow()
        {
            InitializeComponent();
            LoadRequests();
        }
        private void LoadRequests()
        {
            try
            {
                dgRequests.ItemsSource =
                    passwordResetDatabase
                        .GetRequests()
                        .DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки заявок:\n" +
                    ex.Message);
            }
        }
        private void btnChangePassword_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите заявку.");
                return;
            }
            if (string.IsNullOrWhiteSpace(
                txtNewPassword.Password))
            {
                MessageBox.Show(
                    "Введите новый пароль.");
                return;
            }
            DataRowView row =
                (DataRowView)dgRequests.SelectedItem;
            int requestId =
                Convert.ToInt32(
                    row["request_id"]);
            int userId =
                Convert.ToInt32(
                    row["user_id"]);
            try
            {
                passwordResetDatabase.CompleteRequest(
                    requestId,
                    userId,
                    txtNewPassword.Password);
                MessageBox.Show(
                    "Пароль успешно изменён.\n" +
                    "Аккаунт пользователя снова активен.",
                    "Готово",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                txtNewPassword.Clear();
                LoadRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка смены пароля:\n" +
                    ex.Message);
            }
        }
    }
}