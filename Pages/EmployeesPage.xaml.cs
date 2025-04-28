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
                string fullName = selectedEmployee.GetType().GetProperty("EmployeeFullName")?.GetValue(selectedEmployee) as string;

                if (!string.IsNullOrEmpty(fullName))
                {
                    try
                    {
                        Employees employeeToEdit = Entities.GetContext().Employees
                            .FirstOrDefault(emp => (emp.FirstName + " " + emp.LastName) == fullName); 

                        if (employeeToEdit != null)
                        {
                            NavigationService.Navigate(new AddEditEmployeePage(employeeToEdit));
                        }
                        else
                        {
                            MessageBox.Show("Сотрудник не найден.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка во время редактирования: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Не удается получить полное имя выбранного сотрудника.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для редактирования.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PagesEdit.AddEditEmployeePage(null));
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = DataGridEmployees.SelectedItem;

            if (selectedEmployee != null)
            {
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
                            var employee = Entities.GetContext().Employees
                                .FirstOrDefault(emp => (emp.FirstName + " " + emp.LastName) == fullName);

                            if (employee != null)
                            {
                                Entities.GetContext().Employees.Remove(employee);
                                Entities.GetContext().SaveChanges();
                                LoadData(); 
                            }
                            else
                            {
                                MessageBox.Show("Сотрудник, не найденный в базе данных для удаления.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // Modified Message
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Error",
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

        private void UpdateEmployees()
        {
            var currentEmployees = Entities.GetContext().Employees.ToList();

            currentEmployees = currentEmployees.Where(x =>
                x.FirstName.ToLower().Contains(SearchEmployeeName.Text.ToLower()) ||
                x.LastName.ToLower().Contains(SearchEmployeeName.Text.ToLower()) ||
                x.Position.ToLower().Contains(SearchEmployeeName.Text.ToLower())).ToList();

            if (SortEmployeeBy.SelectedIndex == 0)
                currentEmployees = currentEmployees.OrderBy(x => x.FirstName).ToList();
            else if (SortEmployeeBy.SelectedIndex == 1)
                currentEmployees = currentEmployees.OrderByDescending(x => x.FirstName).ToList();
            else if (SortEmployeeBy.SelectedIndex == 2)
                currentEmployees = currentEmployees.OrderBy(x => x.Position).ToList();
            else if (SortEmployeeBy.SelectedIndex == 3)
                currentEmployees = currentEmployees.OrderByDescending(x => x.Position).ToList();

            DataGridEmployees.ItemsSource = currentEmployees;
        }

        private void SearchEmployeeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEmployees();
        }

        private void SortEmployeeBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEmployees();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchEmployeeName.Text = "";
            SortEmployeeBy.SelectedIndex = -1;
            UpdateEmployees();
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
