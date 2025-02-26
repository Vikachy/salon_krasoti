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
            // Используем LINQ-запрос для загрузки данных
            var sales = from sale in Entities.GetContext().Sales
                        join employee in Entities.GetContext().Employees on sale.EmployeeID equals employee.EmployeeID
                        join service in Entities.GetContext().Services on sale.ServiceID equals service.ServiceID
                        select new
                        {
                            SaleID = sale.SaleID,
                            EmployeeName = employee.FirstName + " " + employee.LastName,
                            ServiceName = service.ServiceName,
                            SaleDate = sale.SaleDate,
                            QuantitySold = sale.QuantitySold
                        };

            // Привязываем данные к DataGrid
            DataGridSales.ItemsSource = sales.ToList();
        }

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления продажи
            MessageBox.Show("Добавление продажи");
        }
    }
}