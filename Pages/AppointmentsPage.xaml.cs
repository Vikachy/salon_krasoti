using salon_krasoti.PagesEdit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

namespace salon_krasoti.Pages
{
    /// <summary>
    /// Логика взаимодействия для AppointmentsPage.xaml
    /// </summary>
    public partial class AppointmentsPage : Page
    {
        public AppointmentsPage()
        {
            InitializeComponent();
            DataGridAppointments.ItemsSource = Entities.GetContext().Appointments
            .Include(a => a.Clients)
            .Include(a => a.Employees)
            .Include(a => a.Services)
            .Include(a => a.Promotions)
            .ToList();
        }


        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditAppointmentPage(null));
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную запись
            var selectedAppointment = (Appointments)DataGridAppointments.SelectedItem;

            if (selectedAppointment != null)
            {
                // Открываем страницу редактирования записи
                NavigationService.Navigate(new PagesEdit.AddEditAppointmentPage(selectedAppointment));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную запись
            var selectedAppointment = (Appointments)DataGridAppointments.SelectedItem;

            if (selectedAppointment != null)
            {
                // Подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем запись из базы данных
                        var context = Entities.GetContext();
                        context.Appointments.Remove(selectedAppointment);
                        context.SaveChanges();

                        DataGridAppointments.ItemsSource = Entities.GetContext().Appointments
                        .Include(a => a.Clients)
                        .Include(a => a.Employees)
                        .Include(a => a.Services)
                        .Include(a => a.Promotions)
                        .ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновляем данные из базы данных
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DataGridAppointments.ItemsSource = Entities.GetContext().Appointments
                .Include(a => a.Clients)
                .Include(a => a.Employees)
                .Include(a => a.Services)
                .Include(a => a.Promotions)
                .ToList();
            }
        }
    }
}
