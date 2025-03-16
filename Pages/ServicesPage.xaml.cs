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
    /// Логика взаимодействия для ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : Page
    {
        public ServicesPage()
        {
            InitializeComponent();
            DataGridServices.ItemsSource = Entities.GetContext().Services.ToList();
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditServicePage(null));
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridServices.SelectedItem is Services selectedService)
            {
                NavigationService.Navigate(new PagesEdit.AddEditServicePage(selectedService));
            }
            else
            {
                MessageBox.Show("Выберите услугу для редактирования.");
            }
        }

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridServices.SelectedItem is Services selectedService)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту услугу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Entities.GetContext().Services.Remove(selectedService);
                        Entities.GetContext().SaveChanges();
                        MessageBox.Show("Услуга удалена!");
                        DataGridServices.ItemsSource = Entities.GetContext().Services.ToList(); // Обновляем данные
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления.");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DataGridServices.ItemsSource = Entities.GetContext().Services.ToList();
            }
        }
    }
}
