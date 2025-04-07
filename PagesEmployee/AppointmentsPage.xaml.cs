using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace salon_krasoti.PagesEmployee
{
    public partial class AppointmentsPage : Page
    {
        private readonly int _employeeId;
        private List<Appointments> _allAppointments;

        public AppointmentsPage(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                using (var context = new Entities())
                {
                    _allAppointments = context.Appointments
                        .Where(a => a.EmployeeID == _employeeId)
                        .Include(a => a.Clients)
                        .Include(a => a.Services)
                        .ToList();

                    DataGridAppointments.ItemsSource = _allAppointments.Select(a => new
                    {
                        a.AppointmentID,
                        AppointmentDateTime = a.AppointmentDateTime.ToString("dd.MM.yyyy HH:mm"),
                        ClientName = $"{a.Clients.FirstName} {a.Clients.LastName}",
                        ServiceName = a.Services.ServiceName,
                        a.Status
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке записей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddEditAppointmentPage(_employeeId));
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = DataGridAppointments.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите запись для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new Entities())
            {
                var appointment = context.Appointments.Find(selectedItem.AppointmentID);
                if (appointment != null)
                {
                    NavigationService?.Navigate(new AddEditAppointmentPage(_employeeId, appointment));
                }
            }
        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = DataGridAppointments.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите запись для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new Entities())
                    {
                        var appointment = context.Appointments.Find(selectedItem.AppointmentID);
                        if (appointment != null)
                        {
                            context.Appointments.Remove(appointment);
                            context.SaveChanges();
                            LoadAppointments();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                LoadAppointments();
            }
        }

    }
}