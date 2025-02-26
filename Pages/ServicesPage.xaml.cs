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
            // Логика добавления услуги
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridServices.SelectedItem is Services selectedService)
            {
                // Логика редактирования услуги
            }
        }
    }
}
