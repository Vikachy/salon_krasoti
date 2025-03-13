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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace salon_krasoti.PagesEdit
{
    /// <summary>
    /// Логика взаимодействия для AddEditClientPage.xaml
    /// </summary>
    public partial class AddEditClientPage : Page
    {
        private Clients _client;

        public AddEditClientPage(Clients client = null)
        {
            InitializeComponent();
            _client = client ?? new Clients();

            if (client != null)
            {
                FirstNameTextBox.Text = client.FirstName;
                LastNameTextBox.Text = client.LastName;
                PhoneTextBox.Text = client.Phone;
                EmailTextBox.Text = client.Email;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.FirstName = FirstNameTextBox.Text;
                _client.LastName = LastNameTextBox.Text;
                _client.Phone = PhoneTextBox.Text;
                _client.Email = EmailTextBox.Text;

                var context = Entities.GetContext();
                if (_client.ClientID == 0)
                {
                    context.Clients.Add(_client);
                }
                context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}

