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
    /// Логика взаимодействия для AddEditAppointmentPage.xaml
    /// </summary>
    public partial class AddEditAppointmentPage : Page
    {
        private Appointments _currentAppointment;

        public AddEditAppointmentPage(Appointments selectedAppointment)
        {
            InitializeComponent();

            _currentAppointment = selectedAppointment ?? new Appointments();

            // Загружаем данные для ComboBox
            LoadComboBoxData();

            // Если это редактирование, заполняем поля
            if (selectedAppointment != null)
            {
                ComboBoxClients.SelectedItem = _currentAppointment.Clients;
                ComboBoxEmployees.SelectedItem = _currentAppointment.Employees;
                ComboBoxServices.SelectedItem = _currentAppointment.Services;
                DatePickerAppointment.SelectedDate = _currentAppointment.AppointmentDateTime;
                TextBoxTime.Text = _currentAppointment.AppointmentDateTime.ToString("HH:mm");
                TextBoxStatus.Text = _currentAppointment.Status;
                ComboBoxPromotions.SelectedItem = _currentAppointment.Promotions;
            }
        }



        private void LoadComboBoxData()
        {
            var context = Entities.GetContext();

            ComboBoxClients.ItemsSource = context.Clients.ToList();
            ComboBoxEmployees.ItemsSource = context.Employees.ToList();
            ComboBoxServices.ItemsSource = context.Services.ToList();
            ComboBoxPromotions.ItemsSource = context.Promotions.ToList();
        }

        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = Entities.GetContext();

                if (!TimeSpan.TryParse(TextBoxTime.Text, out var time))
                {
                    MessageBox.Show("Введите время в формате HH:mm.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _currentAppointment.ClientID = ((Clients)ComboBoxClients.SelectedItem).ClientID;
                _currentAppointment.EmployeeID = ((Employees)ComboBoxEmployees.SelectedItem).EmployeeID;
                _currentAppointment.ServiceID = ((Services)ComboBoxServices.SelectedItem).ServiceID;
                _currentAppointment.AppointmentDateTime = DatePickerAppointment.SelectedDate.Value + time;
                _currentAppointment.Status = TextBoxStatus.Text;
                _currentAppointment.PromotionID = ((Promotions)ComboBoxPromotions.SelectedItem)?.PromotionID;

                if (_currentAppointment.AppointmentID == 0) // Если это новая запись
                {
                    context.Appointments.Add(_currentAppointment);
                }

                context.SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
