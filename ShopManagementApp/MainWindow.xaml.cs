using System.Windows;
using ShopManagementApp.Windows;

namespace ShopManagementApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Открытие окна клиентов
        private void btnClients_Click(
            object sender,
            RoutedEventArgs e)
        {
            ClientsWindow clientsWindow =
                new ClientsWindow();
            clientsWindow.Owner = this;
            clientsWindow.ShowDialog();
        }

        // Открытие окна товаров
        private void btnProducts_Click(
            object sender,
            RoutedEventArgs e)
        {
            ProductsWindow productsWindow =
                new ProductsWindow();
            productsWindow.Owner = this;
            productsWindow.ShowDialog();
        }

        // Открытие окна заказов
        private void btnOrders_Click(
            object sender,
            RoutedEventArgs e)
        {
            OrdersWindow ordersWindow =
                new OrdersWindow();
            ordersWindow.Owner = this;
            ordersWindow.ShowDialog();
        }
    }
}