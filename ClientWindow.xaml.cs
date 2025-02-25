using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace salon_krasoti
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private UserAccounts _user;

        public ClientWindow(UserAccounts user)
        {
            InitializeComponent();
            _user = user;
            MainFrame.Navigate(new Pages.AppointmentsPage());
        }
        private void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
            MainFrame.NavigationService.RemoveBackEntry();
        }

        // Обработчики кнопок
        private void Appointments_Click(object sender, RoutedEventArgs e) => NavigateToPage(new Pages.AppointmentsPage());
        private void Services_Click(object sender, RoutedEventArgs e) => NavigateToPage(new Pages.ServicesPage());
        private void MyReviews_Click(object sender, RoutedEventArgs e) => NavigateToPage(new Pages.ReviewsPage());


        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
