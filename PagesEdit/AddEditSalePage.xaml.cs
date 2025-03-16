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
    /// Логика взаимодействия для AddEditSalePage.xaml
    /// </summary>
    public partial class AddEditSalePage : Page
    {
        private Sales _currentSale = new Sales();

        public AddEditSalePage(Sales selectedSale = null)
        {
            InitializeComponent();

            if (selectedSale != null)
            {
                _currentSale = selectedSale;
            }

            DataContext = _currentSale;
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            ComboBoxEmployees.ItemsSource = Entities.GetContext().Employees.ToList();
            ComboBoxEmployees.DisplayMemberPath = "EmployeeFullName";
            ComboBoxServices.ItemsSource = Entities.GetContext().Services.ToList();
        }

        private void SaveSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentSale.SaleID == 0)
                {
                    Entities.GetContext().Sales.Add(_currentSale);
                }

                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены успешно!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
