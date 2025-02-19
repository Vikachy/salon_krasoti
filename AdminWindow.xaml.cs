using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace salon_krasoti
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private UserAccounts _user;

        public AdminWindow(UserAccounts user)
        {
            InitializeComponent();
            _user = user;
            MainFrame.Navigate(new Pages.ClientsPage());
        }

        private void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
            => NavigateToPage(new Pages.ClientsPage());

        private void Employees_Click(object sender, RoutedEventArgs e)
            => NavigateToPage(new Pages.EmployeesPage());

        private void Services_Click(object sender, RoutedEventArgs e)
            => NavigateToPage(new Pages.ServicesPage());

        private void Products_Click(object sender, RoutedEventArgs e)
            => NavigateToPage(new Pages.ProductsPage());

        private void Appointments_Click(object sender, RoutedEventArgs e)
            => NavigateToPage(new Pages.AppointmentsPage());

        private void Promotions_Click(object sender, RoutedEventArgs e)
            => NavigateToPage(new Pages.PromotionsPage());

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
