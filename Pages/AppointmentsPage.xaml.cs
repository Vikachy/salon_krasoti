using salon_krasoti.PagesEdit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace salon_krasoti.Pages
{
    public partial class AppointmentsPage : Page
    {
        private List<Appointments> _allAppointments;

        public AppointmentsPage()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            _allAppointments = Entities.GetContext().Appointments
                .Include(a => a.Clients)
                .Include(a => a.Employees)
                .Include(a => a.Services)
                .Include(a => a.Promotions)
                .ToList();
            UpdateAppointments();
        }

        private void UpdateAppointments()
        {
            var currentAppointments = _allAppointments.AsEnumerable(); 

            if (!string.IsNullOrWhiteSpace(SearchAppointment.Text))
            {
                currentAppointments = currentAppointments.Where(a =>
                    a.Clients.FirstName.ToLower().Contains(SearchAppointment.Text.ToLower()) ||
                    a.Clients.LastName.ToLower().Contains(SearchAppointment.Text.ToLower()) ||
                    a.Employees.FirstName.ToLower().Contains(SearchAppointment.Text.ToLower()) ||
                    a.Employees.LastName.ToLower().Contains(SearchAppointment.Text.ToLower()) ||
                    a.Services.ServiceName.ToLower().Contains(SearchAppointment.Text.ToLower()) ||
                    a.Status.ToLower().Contains(SearchAppointment.Text.ToLower()));
            }

            // Сортировка
            if (SortAppointmentBy.SelectedItem != null)
            {
                switch (((ComboBoxItem)SortAppointmentBy.SelectedItem).Content.ToString())
                {
                    case "По дате (↑)":
                        currentAppointments = currentAppointments.OrderBy(a => a.AppointmentDateTime);
                        break;
                    case "По дате (↓)":
                        currentAppointments = currentAppointments.OrderByDescending(a => a.AppointmentDateTime);
                        break;
                    case "По статусу (А-Я)":
                        currentAppointments = currentAppointments.OrderBy(a => a.Status);
                        break;
                    case "По клиенту (А-Я)":
                        currentAppointments = currentAppointments
                            .OrderBy(a => a.Clients.LastName)
                            .ThenBy(a => a.Clients.FirstName);
                        break;
                }
            }

            DataGridAppointments.ItemsSource = currentAppointments.ToList();
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PagesEdit.AddEditAppointmentPage(null));
        }

        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = (Appointments)DataGridAppointments.SelectedItem;

            if (selectedAppointment != null)
            {
                NavigationService.Navigate(new PagesEdit.AddEditAppointmentPage(selectedAppointment));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = (Appointments)DataGridAppointments.SelectedItem;

            if (selectedAppointment != null)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = Entities.GetContext();
                        context.Appointments.Remove(selectedAppointment);
                        context.SaveChanges();
                        LoadAppointments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchAppointment_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAppointments();
        }

        private void SortAppointmentBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAppointments();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchAppointment.Text = "";
            SortAppointmentBy.SelectedIndex = -1;
            UpdateAppointments();
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