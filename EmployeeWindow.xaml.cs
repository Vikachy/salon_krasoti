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
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        private Employees _employee;

        public EmployeeWindow(Employees employee)
        {
            InitializeComponent();
            _employee = employee; // Используем переданный объект Employees
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            try
            {
                if (_employee != null)
                {
                    // Переход на страницу "Мои записи" по умолчанию
                    MainFrame.Navigate(new PagesEmployee.AppointmentsPage(_employee.EmployeeID));
                }
                else
                {
                    MessageBox.Show("Сотрудник не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
            {
                MainFrame.Navigate(new PagesEmployee.AppointmentsPage(_employee.EmployeeID));
            }
        }

        private void Sales_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
            {
                MainFrame.Navigate(new PagesEmployee.SalesPage(_employee.EmployeeID));
            }
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PagesEmployee.ProductsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
