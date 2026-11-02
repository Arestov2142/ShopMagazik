using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ShopManagementApp.Data;

namespace ShopManagementApp.Windows
{
    public partial class ProductsWindow : Window
    {
        // Объект для работы с базой данных
        private readonly Database database = new Database();
        public ProductsWindow()
        {
            InitializeComponent();

            // При открытии окна загружаем товары
            LoadProducts();
        }


        // Загрузка товаров
        private void LoadProducts()
        {
            try
            {
                dgProducts.ItemsSource =
                    database.GetProducts().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки товаров:\n" +
                    ex.Message);
            }
        }


        // Добавление товара
        private void btnAddProduct_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show(
                    "Введите название товара.");

                return;
            }
            if (!decimal.TryParse(
                txtProductPrice.Text,
                out decimal price))
            {
                MessageBox.Show(
                    "Введите корректную цену.");
                return;
            }
            if (!int.TryParse(
                txtProductQuantity.Text,
                out int quantity))
            {
                MessageBox.Show(
                    "Введите корректное количество.");
                return;
            }
            if (price < 0 || quantity < 0)
            {
                MessageBox.Show(
                    "Цена и количество не могут быть отрицательными.");

                return;
            }
            try
            {
                database.AddProduct(
                    txtProductName.Text,
                    price,
                    quantity);
                MessageBox.Show(
                    "Товар успешно добавлен.");
                ClearProductFields();
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка добавления товара:\n" +
                    ex.Message);
            }
        }


        // Изменение товара
        private void btnEditProduct_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите товар.");

                return;
            }
            if (!decimal.TryParse(
                txtProductPrice.Text,
                out decimal price))
            {
                MessageBox.Show(
                    "Введите корректную цену.");
                return;
            }
            if (!int.TryParse(
                txtProductQuantity.Text,
                out int quantity))
            {
                MessageBox.Show(
                    "Введите корректное количество.");
                return;
            }
            DataRowView row =
                (DataRowView)dgProducts.SelectedItem;
            int productId =
                Convert.ToInt32(row["product_id"]);
            try
            {
                database.UpdateProduct(
                    productId,
                    txtProductName.Text,
                    price,
                    quantity);
                MessageBox.Show(
                    "Товар изменён.");
                ClearProductFields();
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка изменения товара:\n" +
                    ex.Message);
            }
        }


        // Удаление товара
        private void btnDeleteProduct_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите товар.");
                return;
            }
            DataRowView row =
                (DataRowView)dgProducts.SelectedItem;
            int productId =
                Convert.ToInt32(row["product_id"]);
            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить выбранный товар?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                database.DeleteProduct(productId);
                MessageBox.Show(
                    "Товар удалён.");
                ClearProductFields();
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Невозможно удалить товар.\n" +
                    "Возможно, товар используется в заказах.\n\n" +
                    ex.Message);
            }
        }


        // Обновление таблицы
        private void btnRefreshProducts_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadProducts();
        }


        // Выбор товара
        private void dgProducts_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem == null)
            {
                return;
            }
            DataRowView row =
                (DataRowView)dgProducts.SelectedItem;
            txtProductName.Text =
                row["product_name"].ToString();
            txtProductPrice.Text =
                row["price"].ToString();
            txtProductQuantity.Text =
                row["quantity_in_stock"].ToString();
        }


        // Очистка полей
        private void ClearProductFields()
        {
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductQuantity.Clear();
            dgProducts.SelectedItem = null;
        }
    }
}