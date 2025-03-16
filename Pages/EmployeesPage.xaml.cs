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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var employees = Entities.GetContext().Employees
                            .ToList() 
                            .Select(e => new
                            {
                                e.EmployeeFullName, 
                                e.Position,
                                e.Phone,
                                e.Email,
                                HireDate = e.HireDate.ToString("dd/MM/yyyy"), 
                                DateOfBirth = e.DateOfBirth.ToString("dd/MM/yyyy") 
                            })
                            .ToList();

            DataGridEmployees.ItemsSource = employees;
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridEmployees.SelectedItem is object selectedEmployee)
            {
                // Get the EmployeeFullName from the selected item using reflection
                string fullName = selectedEmployee.GetType().GetProperty("EmployeeFullName")?.GetValue(selectedEmployee) as string;

                if (!string.IsNullOrEmpty(fullName))
                {
                    try
                    {
                        // Use a different variable name here (like 'employeeToEdit') to avoid the CS0136 error
                        Employees employeeToEdit = Entities.GetContext().Employees
                            .FirstOrDefault(emp => (emp.FirstName + " " + emp.LastName) == fullName); // Doing it this way to avoid LINQ to Entities issue with EmployeeFullName

                        if (employeeToEdit != null)
                        {
                            // Navigate to the edit page with the employee object
                            NavigationService.Navigate(new AddEditEmployeePage(employeeToEdit));
                        }
                        else
                        {
                            MessageBox.Show("Employee not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during edit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unable to get selected employee's Full Name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PagesEdit.AddEditEmployeePage(null));
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            var selectedEmployee = DataGridEmployees.SelectedItem;

            if (selectedEmployee != null)
            {
                // Use reflection to get the value of the FullName property
                var fullName = selectedEmployee.GetType().GetProperty("EmployeeFullName")?.GetValue(selectedEmployee) as string;

                if (!string.IsNullOrEmpty(fullName))
                {
                    var result = MessageBox.Show($"Удалить сотрудника {fullName}?",
                                                "Подтверждение удаления",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            // Find the employee in the database - CHANGED THIS
                            var employee = Entities.GetContext().Employees
                                .FirstOrDefault(emp => (emp.FirstName + " " + emp.LastName) == fullName);

                            if (employee != null)
                            {
                                // Delete the employee
                                Entities.GetContext().Employees.Remove(employee);
                                Entities.GetContext().SaveChanges();
                                LoadData(); // Refresh data
                            }
                            else
                            {
                                MessageBox.Show("Employee not found in the database for deletion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // Modified Message
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error during deletion: {ex.Message}", "Error", // Modified Message
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadData();
            }
        }
    }
}
