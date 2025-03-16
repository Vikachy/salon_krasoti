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
    /// Логика взаимодействия для AddEditServicePage.xaml
    /// </summary>
    public partial class AddEditServicePage : Page
    {
        private Services _currentService = new Services();

        public AddEditServicePage(Services selectedService = null)
        {
            InitializeComponent();

            if (selectedService != null)
            {
                _currentService = selectedService;
                ServiceNameTextBox.Text = _currentService.ServiceName;
                PriceTextBox.Text = _currentService.Price.ToString();
                DurationTextBox.Text = _currentService.Duration.ToString();
            }

            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentService.ServiceName = ServiceNameTextBox.Text;
                _currentService.Price = decimal.Parse(PriceTextBox.Text);
                _currentService.Duration = int.Parse(DurationTextBox.Text);

                if (_currentService.ServiceID == 0)
                {
                    Entities.GetContext().Services.Add(_currentService);
                }

                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
