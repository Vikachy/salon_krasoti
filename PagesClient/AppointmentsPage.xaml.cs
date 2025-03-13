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
                        ServiceName = a.Services.ServiceName, // Используем навигационное свойство Service
                        EmployeeName = a.Employees.FirstName + " " + a.Employees.LastName, // Используем навигационное свойство Employee
                        Status = a.Status
                    })
                    .ToList();

                // Отладочный вывод
                foreach (var appointment in appointments)
                {
                    Console.WriteLine($"Appointment: {appointment.AppointmentDateTime}, Service: {appointment.ServiceName}, Employee: {appointment.EmployeeName}, Status: {appointment.Status}");
                }

                // Привязка данных к DataGrid
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

            // Проверяем, что выбранная запись принадлежит текущему клиенту
            if (selectedAppointment.ClientID != _currentClientId)
            {
                MessageBox.Show("Вы не можете оставить отзыв на чужую запись.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаем окно для ввода отзыва
            var reviewWindow = new Window
            {
                Title = "Оставить отзыв",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var grid = new Grid();
            reviewWindow.Content = grid;

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var ratingLabel = new Label { Content = "Рейтинг:" };
            grid.Children.Add(ratingLabel);
            Grid.SetRow(ratingLabel, 0);

            var ratingComboBox = new ComboBox
            {
                ItemsSource = new[] { 1, 2, 3, 4, 5 },
                SelectedIndex = 0 // По умолчанию выбран первый элемент
            };
            grid.Children.Add(ratingComboBox);
            Grid.SetRow(ratingComboBox, 0);
            Grid.SetColumn(ratingComboBox, 1);

            var commentLabel = new Label { Content = "Комментарий:" };
            grid.Children.Add(commentLabel);
            Grid.SetRow(commentLabel, 1);

            var commentTextBox = new TextBox { Height = 50, Width = 200 };
            grid.Children.Add(commentTextBox);
            Grid.SetRow(commentTextBox, 1);
            Grid.SetColumn(commentTextBox, 1);

            var saveButton = new Button { Content = "Оставить отзыв" };
            grid.Children.Add(saveButton);
            Grid.SetRow(saveButton, 2);
            Grid.SetColumn(saveButton, 1);

            saveButton.Click += (senderButton, eButton) =>
            {
                try
                {
                    if (ratingComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите рейтинг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var rating = (int)ratingComboBox.SelectedItem;
                    var comment = commentTextBox.Text;

                    var review = new Reviews
                    {
                        ClientID = _currentClientId,
                        ServiceID = selectedAppointment.ServiceID,
                        Rating = rating,
                        Comment = comment
                    };

                    Entities.GetContext().Reviews.Add(review);
                    Entities.GetContext().SaveChanges();

                    reviewWindow.Close();
                    MessageBox.Show("Отзыв успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            reviewWindow.ShowDialog();
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}


