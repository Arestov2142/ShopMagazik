using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class ClientsWindow : Window
    {
        // Объект для работы с базой данных
        private readonly Database database = new Database();
        public ClientsWindow()
        {
            InitializeComponent();
            // При открытии окна загружаем клиентов
            LoadClients();
        }

        // Загрузка клиентов
        private void LoadClients()
        {
            try
            {
                dgClients.ItemsSource =
                    database.GetClients().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки клиентов:\n" +
                    ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // Добавление клиента
        private void btnAddClient_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                MessageBox.Show(
                    "Введите ФИО клиента.");

                return;
            }
            try
            {
                database.AddClient(
                    txtClientName.Text,
                    txtClientPhone.Text,
                    txtClientEmail.Text,
                    txtClientAddress.Text);

                MessageBox.Show(
                    "Клиент успешно добавлен.");

                ClearClientFields();
                LoadClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка добавления клиента:\n" +
                    ex.Message);
            }
        }


        // Изменение клиента
        private void btnEditClient_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgClients.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите клиента.");
                return;
            }

            DataRowView row =
                (DataRowView)dgClients.SelectedItem;
            int clientId =
                Convert.ToInt32(row["client_id"]);
            try
            {
                database.UpdateClient(
                    clientId,
                    txtClientName.Text,
                    txtClientPhone.Text,
                    txtClientEmail.Text,
                    txtClientAddress.Text);
                MessageBox.Show(
                    "Данные клиента изменены.");
                ClearClientFields();
                LoadClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка изменения клиента:\n" +
                    ex.Message);
            }
        }


        // Удаление клиента
        private void btnDeleteClient_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgClients.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите клиента.");

                return;
            }
            DataRowView row =
                (DataRowView)dgClients.SelectedItem;
            int clientId =
                Convert.ToInt32(row["client_id"]);
            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить выбранного клиента?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                database.DeleteClient(clientId);

                MessageBox.Show(
                    "Клиент удалён.");
                ClearClientFields();
                LoadClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Невозможно удалить клиента.\n" +
                    "Возможно, у него существуют заказы.\n\n" +
                    ex.Message);
            }
        }


        // Обновление таблицы
        private void btnRefreshClients_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadClients();
        }


        // Выбор клиента
        private void dgClients_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgClients.SelectedItem == null)
            {
                return;
            }

            DataRowView row =
                (DataRowView)dgClients.SelectedItem;
            txtClientName.Text =
                row["full_name"].ToString();
            txtClientPhone.Text =
                row["phone"].ToString();
            txtClientEmail.Text =
                row["email"].ToString();
            txtClientAddress.Text =
                row["address"].ToString();
        }


        // Очистка полей
        private void ClearClientFields()
        {
            txtClientName.Clear();
            txtClientPhone.Clear();
            txtClientEmail.Clear();
            txtClientAddress.Clear();
            dgClients.SelectedItem = null;
        }
    }
}