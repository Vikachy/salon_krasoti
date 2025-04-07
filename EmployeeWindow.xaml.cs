using System;
using System.Windows;
using System.Windows.Controls;

namespace salon_krasoti
{
    public partial class EmployeeWindow : Window
    {
        private readonly Employees _currentEmployee;

        public EmployeeWindow(Employees employee)
        {
            InitializeComponent();
            _currentEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            try
            {
                MainFrame.Navigate(new PagesEmployee.AppointmentsPage(_currentEmployee.EmployeeID));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PagesEmployee.AppointmentsPage(_currentEmployee.EmployeeID));
        }

        private void Sales_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PagesEmployee.SalesPage(_currentEmployee.EmployeeID));
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PagesEmployee.ProductsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}