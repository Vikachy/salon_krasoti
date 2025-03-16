using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AddEditPaymentPage.xaml
    /// </summary>
    public partial class AddEditPaymentPage : Page
    {
        private Payments _currentPayment;

        public AddEditPaymentPage(Payments selectedPayment)
        {
            InitializeComponent();

            if (selectedPayment != null)
            {
                _currentPayment = selectedPayment;
                Title = "Редактирование платежа";
            }
            else
            {
                _currentPayment = new Payments();
                Title = "Добавление платежа";
            }

            DataContext = _currentPayment;
            LoadAppointments();
            LoadServices();
        }

        private void LoadAppointments()
        {
            ComboBoxAppointments.ItemsSource = Entities.GetContext().Appointments.ToList();
            ComboBoxAppointments.SelectedValue = _currentPayment.AppointmentID;
        }

        private void LoadServices()
        {
            ComboBoxServices.ItemsSource = Entities.GetContext().Services.ToList();
            ComboBoxServices.SelectedValue = _currentPayment.ServiceID;
        }

        private void ComboBoxServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxServices.SelectedItem is Services selectedService)
            {
                TextBoxAmount.Text = selectedService.Price.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentPayment.AppointmentID = (int)ComboBoxAppointments.SelectedValue;
                _currentPayment.ServiceID = (int)ComboBoxServices.SelectedValue;
                _currentPayment.Amount = decimal.Parse(TextBoxAmount.Text);
                _currentPayment.PaymentDate = DatePickerPaymentDate.SelectedDate ?? DateTime.Now;

                if (_currentPayment.PaymentID == 0)
                {
                    Entities.GetContext().Payments.Add(_currentPayment);
                }

                Entities.GetContext().SaveChanges();
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

