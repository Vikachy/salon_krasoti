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
using System.Text.RegularExpressions;


namespace salon_krasoti.PagesEmployee
{
    /// <summary>
    /// Логика взаимодействия для AddEditAppointmentPage.xaml
    /// </summary>
    public partial class AddEditAppointmentPage : Page
    {
        private Appointments _currentAppointment;
        private Employees _currentEmployee;

        public AddEditAppointmentPage(Employees currentEmployee, Appointments selectedAppointment = null)
        {
            InitializeComponent();
            _currentEmployee = currentEmployee;
            _currentAppointment = selectedAppointment ?? new Appointments();

            LoadData();
        }

        private void LoadData()
        {
            using (var context = new Entities())
            {
                // Загрузка клиентов и услуг
                ClientComboBox.ItemsSource = context.Clients.ToList();
                ComboBoxServices.ItemsSource = context.Services.ToList();

                // Загрузка статусов
                ComboBoxStatus.ItemsSource = new List<string> { "Запланировано", "Отменено", "Завершено" };

                // Если это редактирование, заполняем поля
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
                    // Если это новая запись, устанавливаем статус по умолчанию
                    ComboBoxStatus.SelectedItem = "Запланировано";
                }
            }
        }

        private void TextBoxTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Разрешаем ввод только цифр
            if (!Regex.IsMatch(e.Text, "^[0-9]$"))
            {
                e.Handled = true;
                return;
            }

            if (textBox.Text.Length == 2 && !textBox.Text.Contains(":"))
            {
                textBox.Text += ":";
                textBox.CaretIndex = textBox.Text.Length;
            }

            if (fullText.Length > 5)
            {
                e.Handled = true;
            }
        }

        private void TextBoxTime_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.Key == Key.Back && textBox.Text.Length > 0)
            {
                if (textBox.Text.Length == 4 && textBox.Text[2] == ':')
                {
                    textBox.Text = textBox.Text.Substring(0, 2); // Удаляем ":" и одну цифру
                    textBox.CaretIndex = textBox.Text.Length; // Перемещаем курсор в конец
                    e.Handled = true;
                }
            }
        }

        private void SaveAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения полей
                if (ClientComboBox.SelectedItem == null ||
                    ComboBoxServices.SelectedItem == null ||
                    DatePickerAppointment.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(TextBoxTime.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля!");
                    return;
                }

                // Парсинг времени
                var timeParts = TextBoxTime.Text.Split(':');
                if (timeParts.Length != 2 ||
                    !int.TryParse(timeParts[0], out int hours) ||
                    !int.TryParse(timeParts[1], out int minutes))
                {
                    MessageBox.Show("Некорректный формат времени!");
                    return;
                }

                // Создание DateTime
                var selectedDate = DatePickerAppointment.SelectedDate.Value;
                var appointmentDateTime = selectedDate
                    .AddHours(hours)
                    .AddMinutes(minutes);

                // Проверка дня недели (только будни)
                if (appointmentDateTime.DayOfWeek == DayOfWeek.Saturday ||
                    appointmentDateTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    MessageBox.Show("Запись возможна только в будние дни!");
                    return;
                }

                // Проверка времени (7:00 - 22:00)
                if (hours < 7 || hours >= 22)
                {
                    MessageBox.Show("Запись возможна только с 7:00 до 22:00!");
                    return;
                }

                // Получение выбранного статуса
                var selectedStatus = ComboBoxStatus.SelectedItem as string;
                if (string.IsNullOrEmpty(selectedStatus))
                {
                    MessageBox.Show("Выберите статус записи!");
                    return;
                }

                // Сохранение данных
                _currentAppointment.ClientID = ((Clients)ClientComboBox.SelectedItem).ClientID;
                _currentAppointment.ServiceID = ((Services)ComboBoxServices.SelectedItem).ServiceID;
                _currentAppointment.EmployeeID = _currentEmployee.EmployeeID; // Устанавливаем сотрудника
                _currentAppointment.AppointmentDateTime = appointmentDateTime;
                _currentAppointment.Status = selectedStatus; // Устанавливаем статус

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

                MessageBox.Show("Запись успешно сохранена!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
