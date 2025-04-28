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
            LoadClientIdAndNavigate();
        }

        private void LoadClientIdAndNavigate()
        {
            try
            {
                int? clientId = _user.ClientID; 

                if (clientId.HasValue) 
                {
                    MainFrame.Navigate(new PagesClient.AppointmentsPage(clientId.Value)); 
                }
                else
                {
                    MessageBox.Show("Could not find client for the current user.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
            MainFrame.NavigationService.RemoveBackEntry();
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? clientId = _user.ClientID;  

                if (clientId.HasValue) 
                {
                    MainFrame.Navigate(new PagesClient.AppointmentsPage(clientId.Value)); 
                }
                else
                {
                    MessageBox.Show("Could not find client for the current user.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void Services_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new PagesClient.ServicesPage());
        }

        private void MyReviews_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userAccount = Entities.GetContext().UserAccounts
                    .FirstOrDefault(u => u.Username == _user.Username);

                if (userAccount == null)
                {
                    MessageBox.Show($"Пользователь с Username '{_user.Username}' не найден.");
                    return;
                }

                if (!userAccount.ClientID.HasValue)
                {
                    MessageBox.Show($"Для пользователя '{_user.Username}' не найден ClientID.");
                    return;
                }

                NavigateToPage(new PagesClient.MyReviewsPage(userAccount.ClientID.Value));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void AllReviews_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new PagesClient.AllReviewsPage());
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new PagesClient.ProductsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
