using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class OrdersWindow : Window
    {
        // Объект для работы с базой данных
        private readonly Database database = new Database();
        public OrdersWindow()
        {
            InitializeComponent();
            LoadOrders();
            LoadClientsComboBox();
            LoadProductsComboBox();
        }


        // ============================================
        // ЗАГРУЗКА ЗАКАЗОВ
        // ============================================
        private void LoadOrders()
        {
            try
            {
                dgOrders.ItemsSource =
                    database.GetOrders().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки заказов:\n" +
                    ex.Message);
            }
        }


        // ============================================
        // ДОБАВЛЕНИЕ ЗАКАЗА
        // ============================================
        private void btnAddOrder_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (cmbClients.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите клиента.");
                return;
            }
            if (cmbProducts.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите товар.");
                return;
            }
            if (!int.TryParse(
                txtOrderQuantity.Text,
                out int quantity))
            {
                MessageBox.Show(
                    "Введите корректное количество.");
                return;
            }
            if (quantity <= 0)
            {
                MessageBox.Show(
                    "Количество должно быть больше нуля.");
                return;
            }
            ComboBoxItem statusItem =
                (ComboBoxItem)cmbOrderStatus.SelectedItem;
            string status =
                statusItem.Content.ToString() ?? "Новый";
            int clientId =
                Convert.ToInt32(
                    cmbClients.SelectedValue);
            int productId =
                Convert.ToInt32(
                    cmbProducts.SelectedValue);
            try
            {
                database.AddOrder(
                    clientId,
                    productId,
                    quantity,
                    status);
                MessageBox.Show(
                    "Заказ успешно создан.");
                ClearOrderFields();
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка создания заказа:\n" +
                    ex.Message);
            }
        }


        // ============================================
        // ИЗМЕНЕНИЕ ЗАКАЗА
        // ============================================
        private void btnEditOrder_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите заказ.");
                return;
            }
            if (cmbClients.SelectedValue == null ||
                cmbProducts.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите клиента и товар.");
                return;
            }
            if (!int.TryParse(
                txtOrderQuantity.Text,
                out int quantity))
            {
                MessageBox.Show(
                    "Введите корректное количество.");
                return;
            }
            if (quantity <= 0)
            {
                MessageBox.Show(
                    "Количество должно быть больше нуля.");
                return;
            }
            DataRowView row =
                (DataRowView)dgOrders.SelectedItem;
            int orderId =
                Convert.ToInt32(
                    row["order_id"]);
            int clientId =
                Convert.ToInt32(
                    cmbClients.SelectedValue);
            int productId =
                Convert.ToInt32(
                    cmbProducts.SelectedValue);
            ComboBoxItem statusItem =
                (ComboBoxItem)cmbOrderStatus.SelectedItem;
            string status =
                statusItem.Content.ToString() ?? "Новый";
            try
            {
                database.UpdateOrder(
                    orderId,
                    clientId,
                    productId,
                    quantity,
                    status);
                MessageBox.Show(
                    "Заказ изменён.");
                ClearOrderFields();
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка изменения заказа:\n" +
                    ex.Message);
            }
        }


        // ============================================
        // УДАЛЕНИЕ ЗАКАЗА
        // ============================================

        private void btnDeleteOrder_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите заказ.");

                return;
            }
            DataRowView row =
                (DataRowView)dgOrders.SelectedItem;
            int orderId =
                Convert.ToInt32(
                    row["order_id"]);
            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить выбранный заказ?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                database.DeleteOrder(orderId);
                MessageBox.Show(
                    "Заказ удалён.");
                ClearOrderFields();
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка удаления заказа:\n" +
                    ex.Message);
            }
        }


        // ============================================
        // ВЫБОР ЗАКАЗА В ТАБЛИЦЕ
        // ============================================

        private void dgOrders_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgOrders.SelectedItem == null)
            {
                return;
            }
            DataRowView row =
                (DataRowView)dgOrders.SelectedItem;
            cmbClients.SelectedValue =
                Convert.ToInt32(
                    row["client_id"]);
            cmbProducts.SelectedValue =
                Convert.ToInt32(
                    row["product_id"]);
            txtOrderQuantity.Text =
                row["quantity"].ToString();
            string status =
                row["status"].ToString() ?? "";
            foreach (ComboBoxItem item
                in cmbOrderStatus.Items)
            {
                if (item.Content.ToString() == status)
                {
                    cmbOrderStatus.SelectedItem = item;
                    break;
                }
            }
        }


        // ============================================
        // ОБНОВЛЕНИЕ
        // ============================================
        private void btnRefreshOrders_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadOrders();
            LoadClientsComboBox();
            LoadProductsComboBox();
        }


        // ============================================
        // ОЧИСТКА ПОЛЕЙ
        // ============================================

        private void ClearOrderFields()
        {
            cmbClients.SelectedIndex = -1;
            cmbProducts.SelectedIndex = -1;
            txtOrderQuantity.Clear();
            cmbOrderStatus.SelectedIndex = 0;
            dgOrders.SelectedItem = null;
        }


        // ============================================
        // СПИСОК КЛИЕНТОВ
        // ============================================

        private void LoadClientsComboBox()
        {
            try
            {
                cmbClients.ItemsSource =
                    database
                    .GetClientsForComboBox()
                    .DefaultView;
                cmbClients.DisplayMemberPath =
                    "full_name";
                cmbClients.SelectedValuePath =
                    "client_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки клиентов:\n" +
                    ex.Message);
            }
        }


        // ============================================
        // СПИСОК ТОВАРОВ
        // ============================================

        private void LoadProductsComboBox()
        {
            try
            {
                cmbProducts.ItemsSource =
                    database
                    .GetProductsForComboBox()
                    .DefaultView;
                cmbProducts.DisplayMemberPath =
                    "product_name";
                cmbProducts.SelectedValuePath =
                    "product_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки товаров:\n" +
                    ex.Message);
            }
        }
    }
}