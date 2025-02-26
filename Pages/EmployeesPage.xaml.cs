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
            // Загружаем данные из базы и преобразуем их в список анонимных объектов
            var employees = Entities.GetContext().Employees
                .AsEnumerable() // Переключаемся на LINQ to Objects
                .Select(e => new
                {
                    FullName = $"{e.FirstName} {e.LastName}", // Формируем ФИО
                    e.Position,
                    e.Phone,
                    e.Email,
                    HireDate = e.HireDate.ToString("dd/MM/yyyy"), // Форматируем дату
                    DateOfBirth = e.DateOfBirth.ToString("dd/MM/yyyy") // Форматируем дату
                })
                .ToList();

            DataGridEmployees.ItemsSource = employees;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления сотрудника
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент
            var selectedEmployee = DataGridEmployees.SelectedItem;

            if (selectedEmployee != null)
            {
                // Используем рефлексию для получения значения свойства FullName
                var fullName = selectedEmployee.GetType().GetProperty("FullName")?.GetValue(selectedEmployee) as string;

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
                            // Находим сотрудника в базе данных
                            var employee = Entities.GetContext().Employees
                                .FirstOrDefault(emp => $"{emp.FirstName} {emp.LastName}" == fullName);

                            if (employee != null)
                            {
                                // Удаляем сотрудника
                                Entities.GetContext().Employees.Remove(employee);
                                Entities.GetContext().SaveChanges();
                                LoadData(); // Обновляем данные
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
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
    }
}
