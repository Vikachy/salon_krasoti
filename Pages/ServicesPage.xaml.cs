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
using System.Xaml;

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
            LoadServices();
        }

        private void LoadServices()
        {
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
                MessageBox.Show("Выберите услугу для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridServices.SelectedItem is Services selectedService)
            {
                var message = $"Вы уверены, что хотите удалить услугу \"{selectedService.ServiceName}\"?";

                if (MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = Entities.GetContext();
                        var serviceToDelete = context.Services.Find(selectedService.ServiceID);
                        if (serviceToDelete != null)
                        {
                            context.Services.Remove(serviceToDelete);
                            context.SaveChanges();
                            MessageBox.Show($"Услуга \"{selectedService.ServiceName}\" успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadServices(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении услуги: {ex.Message}\n\nПодробности: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void UpdateServices()
        {
            var currentServices = Entities.GetContext().Services.ToList(); ;

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(SearchServiceName.Text))
            {
                currentServices = currentServices.Where(s =>
                    s.ServiceName.ToLower().Contains(SearchServiceName.Text.ToLower()) ||
                    s.Price.ToString().Contains(SearchServiceName.Text) ||
                    s.Duration.ToString().Contains(SearchServiceName.Text)).ToList();
            }

            // Сортировка
            if (SortServiceBy.SelectedItem != null)
            {
                switch (((ComboBoxItem)SortServiceBy.SelectedItem).Content.ToString())
                {
                    case "По названию (А-Я)":
                        currentServices = currentServices.OrderBy(s => s.ServiceName).ToList();
                        break;
                    case "По названию (Я-А)":
                        currentServices = currentServices.OrderByDescending(s => s.ServiceName).ToList();
                        break;
                    case "По цене (↑)":
                        currentServices = currentServices.OrderBy(s => s.Price).ToList();
                        break;
                    case "По цене (↓)":
                        currentServices = currentServices.OrderByDescending(s => s.Price).ToList();
                        break;
                }
            }

            DataGridServices.ItemsSource = currentServices;
        }

        private void SearchServiceName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void SortServiceBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchServiceName.Text = "";
            SortServiceBy.SelectedIndex = -1;
            UpdateServices();
        }


        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadServices();
            }
        }
    }
}