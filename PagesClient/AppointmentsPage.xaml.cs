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
    public partial class AppointmentsPage : Page
    {
        private int _currentClientId; // ID текущего клиента

        public AppointmentsPage(int clientId)
        {
            InitializeComponent();
            _currentClientId = clientId;
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                // Загрузка записей для текущего клиента
                var appointments = Entities.GetContext().Appointments
                    .Where(a => a.ClientID == _currentClientId)
                    .Select(a => new
                    {
                        AppointmentID = a.AppointmentID,
                        AppointmentDateTime = a.AppointmentDateTime,
                        ServiceID = a.ServiceID, 
                        ServiceName = a.Services.ServiceName, 
                        EmployeeName = a.Employees.FirstName + " " + a.Employees.LastName, 
                        Status = a.Status
                    })
                    .ToList();

                DataGridAppointments.ItemsSource = appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке записей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LeaveReview_Click(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = DataGridAppointments.SelectedItem as dynamic;
            if (selectedAppointment == null)
            {
                MessageBox.Show("Выберите запись для оставления отзыва.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NavigationService.Navigate(new AddReviewPage(_currentClientId, selectedAppointment.ServiceID));
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesClient.AddAppointmentPage(_currentClientId));
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


