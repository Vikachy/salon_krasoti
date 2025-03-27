using salon_krasoti.PagesClient;
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

namespace salon_krasoti.PagesEmployee
{
    /// <summary>
    /// Логика взаимодействия для AppointmentsPage.xaml
    /// </summary>
     public partial class AppointmentsPage : Page
    {
        private int _currentEmployeeId;
        private Employees _currentEmployee;

        public AppointmentsPage(int employeeId)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                // Загрузка записей для текущего сотрудника
                var appointments = Entities.GetContext().Appointments
                    .Where(a => a.EmployeeID == _currentEmployeeId)
                    .Select(a => new
                    {
                        AppointmentID = a.AppointmentID,
                        AppointmentDateTime = a.AppointmentDateTime,
                        ClientName = a.Clients.FirstName + " " + a.Clients.LastName, // Имя клиента
                        ServiceName = a.Services.ServiceName, // Название услуги
                        Status = a.Status // Статус записи
                    })
                    .ToList();

                // Привязка данных к DataGrid
                DataGridAppointments.ItemsSource = appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке записей: {ex.Message}");
            }
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentEmployee = Entities.GetContext().Employees
                    .FirstOrDefault(emp => emp.EmployeeID == _currentEmployeeId);

                NavigationService.Navigate(new AddEditAppointmentPage(currentEmployee));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedAppointment = DataGridAppointments.SelectedItem as dynamic;
                if (selectedAppointment == null)
                {
                    MessageBox.Show("Выберите запись для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int appointmentId = selectedAppointment.AppointmentID;

                // Находим запись в базе данных
                var appointmentToDelete = Entities.GetContext().Appointments
                    .FirstOrDefault(a => a.AppointmentID == appointmentId);

                if (appointmentToDelete == null)
                {
                    MessageBox.Show("Запись не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Entities.GetContext().Appointments.Remove(appointmentToDelete);
                    Entities.GetContext().SaveChanges();
                    LoadAppointments(); // Обновляем данные после удаления
                    MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
            }
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем выбранный анонимный объект
                var selectedAppointment = DataGridAppointments.SelectedItem as dynamic;
                if (selectedAppointment == null)
                {
                    MessageBox.Show("Выберите запись для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получаем ID выбранной записи
                int appointmentId = selectedAppointment.AppointmentID;

                // Находим запись в базе данных
                var appointmentToEdit = Entities.GetContext().Appointments
                    .FirstOrDefault(a => a.AppointmentID == appointmentId);

                if (appointmentToEdit == null)
                {
                    MessageBox.Show("Запись не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                NavigationService.Navigate(new AddEditAppointmentPage(_currentEmployee, appointmentToEdit));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании записи: {ex.Message}");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadAppointments();
            }
        }
    }
}

