using salon_krasoti.PagesEdit;
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

namespace salon_krasoti.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            DataGridClients.ItemsSource = Entities.GetContext().Clients.ToList();
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditClientPage(null));
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = DataGridClients.SelectedItem as Clients;
            if (selectedClient != null)
            {
                NavigationService.Navigate(new PagesEdit.AddEditClientPage(selectedClient));
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.");
            }
        }


        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridClients.SelectedItem is Clients client)
            {
                DeleteClient(client);
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteClient(Clients client)
        {
            var result = MessageBox.Show($"Удалить клиента {client.FirstName} {client.LastName}?",
                                        "Подтверждение удаления",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем клиента
                    Entities.GetContext().Clients.Remove(client);
                    Entities.GetContext().SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    MessageBox.Show($"Невозможно удалить клиента: {ex.InnerException?.InnerException?.Message}",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DataGridClients.ItemsSource = Entities.GetContext().Clients.ToList();
            }
        }
    }

}

