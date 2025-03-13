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
    /// Логика взаимодействия для AddEditEmployeePage.xaml
    /// </summary>
    public partial class AddEditEmployeePage : Page
    {
        private Employees _employee;

        public AddEditEmployeePage(Employees employee)
        {
            InitializeComponent();
            _employee = employee;

            if (_employee != null)
            {
                // Заполняем поля данными сотрудника, если это редактирование
                TextBoxFirstName.Text = _employee.FirstName;
                TextBoxLastName.Text = _employee.LastName;
                TextBoxPosition.Text = _employee.Position;
                TextBoxPhone.Text = _employee.Phone;
                TextBoxEmail.Text = _employee.Email;
                DatePickerHireDate.SelectedDate = _employee.HireDate;
                DatePickerDateOfBirth.SelectedDate = _employee.DateOfBirth;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = Entities.GetContext();

                if (_employee == null)
                {
                    // Добавление нового сотрудника
                    var newEmployee = new Employees
                    {
                        FirstName = TextBoxFirstName.Text,
                        LastName = TextBoxLastName.Text,
                        Position = TextBoxPosition.Text,
                        Phone = TextBoxPhone.Text,
                        Email = TextBoxEmail.Text,
                        HireDate = DatePickerHireDate.SelectedDate.Value,
                        DateOfBirth = DatePickerDateOfBirth.SelectedDate.Value
                    };
                    context.Employees.Add(newEmployee);
                }
                else
                {
                    // Редактирование существующего сотрудника
                    _employee.FirstName = TextBoxFirstName.Text;
                    _employee.LastName = TextBoxLastName.Text;
                    _employee.Position = TextBoxPosition.Text;
                    _employee.Phone = TextBoxPhone.Text;
                    _employee.Email = TextBoxEmail.Text;
                    _employee.HireDate = DatePickerHireDate.SelectedDate.Value;
                    _employee.DateOfBirth = DatePickerDateOfBirth.SelectedDate.Value;
                }

                context.SaveChanges();
                NavigationService?.GoBack(); // Возврат на предыдущую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack(); // Возврат на предыдущую страницу
        }
    }
}
