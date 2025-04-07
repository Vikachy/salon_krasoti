using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace salon_krasoti.PagesEmployee
{
    public partial class AddEditAppointmentPage : Page
    {
        private Appointments _currentAppointment;
        private int _employeeId;

        public string PageTitle => _currentAppointment.AppointmentID == 0
            ? "Добавление записи"
            : "Редактирование записи";

        public AddEditAppointmentPage(int employeeId, Appointments selectedAppointment = null)
        {
            InitializeComponent();
            _employeeId = employeeId;
            _currentAppointment = selectedAppointment ?? new Appointments
            {
                EmployeeID = employeeId,
                Status = "Запланировано"
            };

            DataContext = this;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new Entities())
                {
                    ClientComboBox.ItemsSource = context.Clients.ToList();
                    ComboBoxServices.ItemsSource = context.Services.ToList();
                    ComboBoxStatus.ItemsSource = new[] { "Запланировано", "Отменено", "Завершено" };

                    if (_currentAppointment.AppointmentID != 0)
                    {
                        ClientComboBox.SelectedValue = _currentAppointment.ClientID;
                        ComboBoxServices.SelectedValue = _currentAppointment.ServiceID;
                        DatePickerAppointment.SelectedDate = _currentAppointment.AppointmentDateTime;
                        TextBoxTime.Text = _currentAppointment.AppointmentDateTime.ToString("HH:mm");
                        ComboBoxStatus.SelectedItem = _currentAppointment.Status;
                    }
                    else
                    {
                        ComboBoxStatus.SelectedIndex = 0;
                        DatePickerAppointment.SelectedDate = DateTime.Today;
                        TextBoxTime.Text = DateTime.Now.ToString("HH:mm");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (ClientComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ComboBoxServices.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DatePickerAppointment.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextBoxTime.Text) || !Regex.IsMatch(TextBoxTime.Text, @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$"))
            {
                MessageBox.Show("Введите корректное время в формате HH:mm!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private DateTime CreateAppointmentDateTime()
        {
            var timeParts = TextBoxTime.Text.Split(':');
            var selectedDate = DatePickerAppointment.SelectedDate.Value;
            return new DateTime(
                selectedDate.Year,
                selectedDate.Month,
                selectedDate.Day,
                int.Parse(timeParts[0]),
                int.Parse(timeParts[1]),
                0);
        }

        private bool ValidateDateTime(DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Запись возможна только в будние дни!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dateTime.Hour < 7 || dateTime.Hour >= 22)
            {
                MessageBox.Show("Запись возможна только с 7:00 до 22:00!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dateTime < DateTime.Now)
            {
                MessageBox.Show("Нельзя создавать записи в прошлом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UpdateAppointmentData(DateTime appointmentDateTime)
        {
            _currentAppointment.ClientID = (int)ClientComboBox.SelectedValue;
            _currentAppointment.ServiceID = (int)ComboBoxServices.SelectedValue;
            _currentAppointment.EmployeeID = _employeeId;
            _currentAppointment.AppointmentDateTime = appointmentDateTime;
            _currentAppointment.Status = ComboBoxStatus.SelectedItem.ToString();
        }

        private void TextBoxTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "^[0-9]$"))
            {
                e.Handled = true;
                return;
            }

            var textBox = (TextBox)sender;
            if (textBox.Text.Length == 2 && !textBox.Text.Contains(":"))
            {
                textBox.Text += ":";
                textBox.CaretIndex = textBox.Text.Length;
            }

            if (textBox.Text.Length >= 5)
            {
                e.Handled = true;
            }
        }

        private void TextBoxTime_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && ((TextBox)sender).Text.Length > 0)
            {
                var textBox = (TextBox)sender;
                if (textBox.Text.Length == 4 && textBox.Text[2] == ':')
                {
                    textBox.Text = textBox.Text.Substring(0, 2);
                    textBox.CaretIndex = textBox.Text.Length;
                    e.Handled = true;
                }
            }
        }

        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var appointmentDateTime = CreateAppointmentDateTime();
            if (!ValidateDateTime(appointmentDateTime)) return;

            UpdateAppointmentData(appointmentDateTime);

            try
            {
                using (var context = new Entities())
                {
                    if (_currentAppointment.AppointmentID == 0)
                    {
                        context.Appointments.Add(_currentAppointment);
                    }
                    else
                    {
                        context.Entry(_currentAppointment).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }

                MessageBox.Show("Запись успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}