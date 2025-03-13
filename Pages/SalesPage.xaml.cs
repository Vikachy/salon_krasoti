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
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        public SalesPage()
        {
            InitializeComponent();
            LoadSales();
        }

        private void LoadSales()
        {
            // Загрузка данных с использованием навигационных свойств
            var sales = Entities.GetContext().Sales
                .Select(s => new
                {
                    SaleID = s.SaleID,
                    EmployeeName = s.Employees.FirstName + " " + s.Employees.LastName, 
                    ServiceName = s.Services.ServiceName, 
                    SaleDate = s.SaleDate,
                    QuantitySold = s.QuantitySold
                })
                .ToList();

            DataGridSales.ItemsSource = sales;
        }

        private void EditSale_Click(object sender, RoutedEventArgs e)
        {

        }

            private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления продажи
            MessageBox.Show("Добавление продажи");
        }

        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}