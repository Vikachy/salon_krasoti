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

namespace salon_krasoti.PagesClient
{
    /// <summary>
    /// Логика взаимодействия для AddAppointmentPage.xaml
    /// </summary>
    public partial class AddAppointmentPage : Page
    {
        private int _clientId;

        public AddAppointmentPage(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                ComboBoxEmployees.ItemsSource = Entities.GetContext().Employees.ToList();
                ComboBoxServices.ItemsSource = Entities.GetContext().Services.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboBoxEmployees.SelectedItem == null || ComboBoxServices.SelectedItem == null || DatePickerAppointment.SelectedDate == null || string.IsNullOrEmpty(TextBoxTime.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedEmployee = (Employees)ComboBoxEmployees.SelectedItem;
                var selectedService = (Services)ComboBoxServices.SelectedItem;
                var appointmentDateTime = DatePickerAppointment.SelectedDate.Value;
                var time = TextBoxTime.Text;

                if (!DateTime.TryParse($"{appointmentDateTime.ToShortDateString()} {time}", out DateTime fullDateTime))
                {
                    MessageBox.Show("Некорректный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Проверка, занят ли сотрудник в это время
                var isEmployeeBusy = Entities.GetContext().Appointments
                    .Any(a => a.EmployeeID == selectedEmployee.EmployeeID && a.AppointmentDateTime == fullDateTime);

                if (isEmployeeBusy)
                {
                    MessageBox.Show("Сотрудник уже занят в это время.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var appointment = new Appointments
                {
                    ClientID = _clientId,
                    EmployeeID = selectedEmployee.EmployeeID,
                    ServiceID = selectedService.ServiceID,
                    AppointmentDateTime = fullDateTime,
                    Status = "Запланировано" 
                };

                Entities.GetContext().Appointments.Add(appointment);
                Entities.GetContext().SaveChanges();

                MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
