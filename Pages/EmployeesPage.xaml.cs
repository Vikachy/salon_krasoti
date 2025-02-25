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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            DataGridEmployees.ItemsSource = Entities.GetContext().Employees.ToList();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления сотрудника
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Логика редактирования сотрудника
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridEmployees.SelectedItem is Employees employee)
            {
                var result = MessageBox.Show($"Удалить сотрудника {employee.FirstName} {employee.LastName}?",
                                            "Подтверждение удаления",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Entities.GetContext().Employees.Remove(employee);
                        Entities.GetContext().SaveChanges();
                        DataGridEmployees.ItemsSource = Entities.GetContext().Employees.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
